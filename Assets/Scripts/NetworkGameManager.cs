using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkGameManager : NetworkBehaviour
{
    public static NetworkGameManager instance;
    public const int MAX_PLAYERS = 12;
    public GameObject playerPrefab;                                 // ���� ���� �� ������ �÷��̾�

    public NetworkList<PlayerData> playerDataNetworkList;           // ��Ʈ��ũ �󿡼� ����ȭ�Ǵ� �÷��̾� ������ ���
    public delegate void OnPlayerDataListChanged();                 // �÷��̾� ������ ����� ����� �� ȣ��Ǵ� ��������Ʈ
    public static OnPlayerDataListChanged onPlayerDataListChanged;

    public GameObject myPlayer; // only set when ingame;

    string username;    // �÷��̾� ����ڸ� ����

    // ���� ���� ����
    public enum GameState // only allow players to join while waiting to start
    {
        WaitingToStart,
        InHub,
        InGame,
        End
    }
    public NetworkVariable<GameState> gameState = new NetworkVariable<GameState>(GameState.WaitingToStart);
    private void Awake()
    {
        // �̱��� �����
        if (NetworkGameManager.instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        playerDataNetworkList = new NetworkList<PlayerData>();
        DontDestroyOnLoad(gameObject);

        username = PlayerPrefs.GetString("USERNAME", "Guest: " + Random.Range(100, 1000));
    }

    private void Start()
    {
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged; // �÷��̾� ������ ����� ���� �̺�Ʈ �ڵ鷯 ����
    }

    public string GetUsername() // �÷��̾� �̸� ��������
    {
        return username;
    }
    public void SetUsername(string _username) // �÷��̾� �̸� �����ϱ�
    {
        if (string.IsNullOrWhiteSpace(_username))
        {
            username = "Guest: " + Random.Range(100, 1000);
        }
        else
        {
             username = _username;
        }

        PlayerPrefs.SetString("USERNAME", username);
    }

    public string GetUsernameFromClientId(ulong _clientId) // Ŭ���̾�Ʈ ID�� ���� ����ڸ� ��������
    {
        foreach (PlayerData playerData in playerDataNetworkList)
        {
            if (playerData.clientId == _clientId)
                return playerData.username.ToString();
        }
        return default;
    }
    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent) // �÷��̾� ������ ����� ����� �� ȣ��
    {
        //Debug.Log("Invoke");
        //Debug.Log(playerDataNetworkList.Count);
        onPlayerDataListChanged?.Invoke();
    }
    public bool IsPlayerIndexConnected(int playerIndex)
    {
        return playerIndex < playerDataNetworkList.Count;
    }
    public PlayerData GetPlayerDataFromIndex(int _playerIndex)
    {
        return playerDataNetworkList[_playerIndex];
    }
    public PlayerData GetPlayerDataFromClientId(ulong clientId)
    {
        foreach (PlayerData playerData in playerDataNetworkList)
        {
            if (playerData.clientId == clientId)
                return playerData;
        }
        return default;
    }
    public PlayerData GetLocalPlayerData()
    {
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }
    public int GetPlayerDataIndexFromClientID(ulong clientId)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            if (playerDataNetworkList[i].clientId == clientId)
                return i;
        }
        return -1;
    }
    public int GetPlayerSkinFromIndex(int _playerIndex) // �÷��̾� ��Ų ���� (���� ����)
    {
        return playerDataNetworkList[_playerIndex].skinIndex;
    }

    public void ChangePlayerSkin(int skinIndex) // �÷��̾� ��Ų ����, ������ RPC�� ȣ���Ͽ� ���� (���� ����)
    {
        ChangePlayerSkinServerRpc(skinIndex);
    }

    [ServerRpc(RequireOwnership = false)]
    void ChangePlayerSkinServerRpc(int skinIndex, ServerRpcParams rpcParams = default) // (���� ����)
    {
        int playerIndex = GetPlayerDataIndexFromClientID(rpcParams.Receive.SenderClientId);
        PlayerData data = playerDataNetworkList[playerIndex];
        data.skinIndex = skinIndex;
        playerDataNetworkList[playerIndex] = data;
    }

    public void StartHost() // ȣ��Ʈ�� �����ϰ�, ���� ���� �ݹ�, Ŭ���̾�Ʈ ���� �� ���� �ݹ��� ����, �κ� �� �ε�
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += Network_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += Network_Server_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += Network_Server_OnClientDisconnectCallback;

        NetworkManager.Singleton.StartHost();
        LoadLobbyJoinedScene();
    }

    private void Network_Server_OnClientDisconnectCallback(ulong _clientId) // �������� Ŭ���̾�Ʈ�� ���� ������ �� ȣ��
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            PlayerData data = playerDataNetworkList[i];
            if (data.clientId == _clientId)
            {
                playerDataNetworkList.RemoveAt(i);
            }
        }

        if (SceneManager.GetActiveScene().name == "MULTIPLAYER_GAME_SCENE")
        {
            //Scoreboard.Instance.ResetScoreboard();
        }
    }
    private void Network_Server_OnClientConnectedCallback(ulong _clientId) // �������� Ŭ���̾�Ʈ�� ����� �� ȣ��
    {
        playerDataNetworkList.Add(new PlayerData
        {
            clientId = _clientId,
            skinIndex = 0
        });
        SetUsernameServerRpc(GetUsername());
    }
    public void StartClient() // Ŭ���̾�Ʈ ����, Ŭ���̾�Ʈ ���� �� ���� �ݹ� ����
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += Network_OnClientDisconnectCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += Network_Client_OnClientConnectedCallback;

        NetworkManager.Singleton.StartClient();
    }

    private void Network_Client_OnClientConnectedCallback(ulong obj) // Ŭ���̾�Ʈ�� ����Ǿ��� �� ȣ��
    {
        SetUsernameServerRpc(GetUsername());
        if (SceneManager.GetActiveScene().name == "MULTIPLAYER_GAME_SCENE")
        {
            //Scoreboard.Instance.ResetScoreboard();
        }
    }
    [ServerRpc(RequireOwnership = false)] // Ŭ���̾�Ʈ�� �������� ���� ��Ʈ��ũ ������Ʈ������ �� ���� RPC�� ȣ���� �� ������ ��Ÿ����.
    void SetUsernameServerRpc(string _username, ServerRpcParams rpcParams = default)
    {
        int playerIndex = GetPlayerDataIndexFromClientID(rpcParams.Receive.SenderClientId);
        PlayerData data = playerDataNetworkList[playerIndex];
        data.username = _username;
        playerDataNetworkList[playerIndex] = data;
    }

    private void Network_OnClientDisconnectCallback(ulong clientId) // Ŭ���̾�Ʈ ������ �����Ǿ��� �� ȣ��
    {
        //Debug.Log("2");
        if (SceneManager.GetSceneByName("MULTIPLAYER_LOBBY_BROWSE_SCENE") == SceneManager.GetActiveScene())
        {
            // failed to connect
            FindObjectOfType<LobbyBrowseUI>().ConnectionFailed(); // ���� ���� �޽��� UI�� ����
        }
        else if (SceneManager.GetSceneByName("MULTIPLAYER_LOBBY_JOINED_SCENE") == SceneManager.GetActiveScene()) // �κ�(���� ��) ��
        {
            // inside a lobby;
            FindObjectOfType<LobbyJoinedUI>().LeaveLobbyPressed(); // �κ� ������
        }
        else
        {
            // ingame
            //UI.instance.EnableHostDisconnectTab();
        }

        //throw new System.NotImplementedException();
    }

    /// <summary>
    /// ���ο� Ŭ���̾�Ʈ�� ������ ������ �õ��� �� �ش� ���� ��û�� �������� ���θ� �����ϴ� �ݹ� �Լ�
    /// </summary>
    /// <param name="connectionApprovalRequest"></param>
    /// <param name="connectionApprovalResponse"></param>
    void Network_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        //Debug.Log("1");
        if (gameState.Value != GameState.WaitingToStart)
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game has already started.";
            return;
        }
        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYERS) // ������ ���̸�
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game is full.";
            return;
        }
        connectionApprovalResponse.Approved = true;
        //connectionApprovalResponse.CreatePlayerObject = true; 
    }

    void LoadLobbyJoinedScene()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("MULTIPLAYER_LOBBY_JOINED_SCENE", LoadSceneMode.Single);
    }

    public void LoadGameScene()
    {
        LobbyManager.instance.DeleteLobby(); // ���� ���� ���� �κ� ��Ͽ� �ȶ߰� �ϴϱ� �׳� ����������.

        //string map = PlayerPrefs.GetString("ZOMBIES_MAP", "LAB");
        NetworkManager.Singleton.SceneManager.LoadScene("MULTIPLAYER_GAME_SCENE", LoadSceneMode.Single);
    }

    public void SpawnPlayers() // server
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) // ����� Ŭ���̾�Ʈ ID ��ȸ
        {
            GameObject player = Instantiate(playerPrefab); // �÷��̾� ������Ʈ ����

            // clientId�� �÷��̾� ������Ʈ�� '����'�� Ŭ���̾�Ʈ ID
            // true�� �÷��̾� ������Ʈ�� ���������� �����ϰڴٴ� ��, �̸� ���� ��� Ŭ���̾�Ʈ�� �� ������Ʈ�� �ν��ϰ� �ȴ�.
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
    }

}
