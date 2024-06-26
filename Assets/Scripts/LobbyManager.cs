using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;

/// <summary>
/// ��Ƽ�÷��̾� ���� �κ� ���� ��� �����ϴ� Ŭ����
/// </summary>
public class LobbyManager : MonoBehaviour
{
    public static LobbyManager instance;                // �̱��� �ν��Ͻ�
    const string KEY_RELAY_JOIN_CODE = "RelayJoinCode"; // �κ� �����Ϳ� ����Ǵ� Relay Join �ڵ带 ���� Ű ���

    Lobby joinedLobby;                                  // �÷��̾ ���� ������ �κ�

    float heartBeatTimer;                               // �κ��� ��Ʈ��Ʈ Ÿ�̸� 
    float heartBeatTimerMax = 15;                       // �κ��� ��Ʈ��Ʈ Ÿ�̸� �ִ밪

    float listLobbiesTimer;                             // �κ� ��� ���� Ÿ�̸�
    float listLobbiesTimerMax = 3;                      // �κ� ��� ���� Ÿ�̸� �ִ밪

    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged; // �κ� ����� ����� �� �߻��ϴ� �̺�Ʈ
    public class OnLobbyListChangedEventArgs : EventArgs    // �̺�Ʈ ���ڷ� �κ� ����� �����ϱ� ���� Ŭ����
    {
        public List<Lobby> lobbyList;
    }
    private void Awake()
    {
        // �̱������� �����
        instance = this;
        DontDestroyOnLoad(gameObject);
        
        // ��Ʈ��Ʈ Ÿ�̸�, �κ� ��� ���� Ÿ�̸� �ִ밪���� �ʱ�ȭ
        heartBeatTimer = heartBeatTimerMax;
        listLobbiesTimer = listLobbiesTimerMax;

        InitializeUnityAuthentication();
    }

    /// <summary>
    /// �͸� ���� �ʱ�ȭ
    /// </summary>
    async void InitializeUnityAuthentication()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized) // Unity ���񽺰� �ʱ�ȭ ���� �ʾ��� ���
        {
            // InitializationOptions ��ü ����
            InitializationOptions options = new InitializationOptions();

            // ������ ����, 100���� 999 ������ ������ ���ڸ� ���ڿ��� ��ȯ�Ͽ� ������ �̸����� ���
            options.SetProfile(UnityEngine.Random.Range(100, 1000).ToString());

            // Unity ���� �ʱ�ȭ, �񵿱������� ó���ǹǷ� await Ű���带 ����Ͽ� �ϷḦ ��ٸ���.
            await UnityServices.InitializeAsync(options);

            // �͸� ����, �񵿱������� ó���ǹǷ� await Ű���带 ����Ͽ� �ϷḦ ��ٸ���.
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    /// <summary>
    /// Relay �Ҵ� ��û
    /// </summary>
    /// <returns></returns>
    async Task<Allocation> AllocateRelay()
    {
        try
        {
            // Relay ���� �ν��Ͻ��� ����Ͽ� �Ҵ� ����
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(NetworkGameManager.MAX_PLAYERS - 1);
            return allocation; // ������ �Ҵ� ��ȯ
        }
        catch (RelayServiceException ex)
        {
            Debug.Log(ex);  // ���� �޼��� ���
            return default; // �⺻ �� ��ȯ
        }
    }

    /// <summary>
    /// �Ҵ�� Relay join �ڵ� ��û
    /// </summary>
    /// <param name="allocation"></param>
    /// <returns></returns>
    async Task<string> GetRelayJoinCode(Allocation allocation) // Task<string> ��ȯ ������ �� �޼��尡 �񵿱������� ���ڿ�(join �ڵ�)�� ��ȯ�� ���� �ǹ�
    {
        try
        {
            // Relay ���� �ν��Ͻ��� ����Ͽ� �Ҵ� ID�� ������� join �ڵ带 �����´�.
            string relayCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return relayCode; // join �ڵ� ��ȯ
        }
        catch (RelayServiceException ex)
        {
            Debug.Log(ex);  // ���� �޼��� ���
            return default; // �⺻ �� ��ȯ
        }
    }

    /// <summary>
    /// Relay join �ڵ带 ����Ͽ� Relay�� ����
    /// </summary>
    /// <param name="joinCode"></param>
    /// <returns></returns>
    async Task<JoinAllocation> JoinRelay(string joinCode)
    {
        try
        {
            // Relay ���� �ν��Ͻ��� ����Ͽ� join �ڵ带 ���� �Ҵ翡 ����
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            return joinAllocation; // ����� �Ҵ� ���� ��ȯ
        }
        catch (RelayServiceException ex)
        {
            Debug.Log(ex); // ���� �޽��� ���
            LobbyBrowseUI.instance.LobbyConnectError(ex.Reason.ToString()); // �κ� ���� ���� �޽��� UI�� ����
            return default; // �⺻ �� ��ȯ

        }
    }

    /// <summary>
    /// �κ� ����, Relay �Ҵ��Ͽ� �κ� ������ ������Ʈ,
    /// ��Ʈ��ũ �Ŵ��� ����, ȣ��Ʈ ����
    /// </summary>
    /// <param name="lobbyName"></param>
    /// <param name="isPrivate"></param>
    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        try
        {
            // �κ� ����
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, NetworkGameManager.MAX_PLAYERS, new CreateLobbyOptions
            {
                IsPrivate = isPrivate // ���� ����
            });

            // Relay �Ҵ� ����
            Allocation allocation = await AllocateRelay();
            string relayJoinCode = await GetRelayJoinCode(allocation); // Relay ������ ������ �� ����� join �ڵ� 

            // �κ� �����͸� ������Ʈ�Ͽ� join �ڵ带 ����
            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                }
            });
            // NetworkManager�� UnityTransport ���� ��ҿ� Relay ���� ������ ����
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));

            // ���� ����(����)�� ȣ��Ʈ�� ����
            NetworkGameManager.instance.StartHost();
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex); // ���� �޽��� ���
            LobbyBrowseUI.instance.LobbyConnectError(ex.Reason.ToString()); // �κ� ���� ���� �޽��� UI�� ����
        }
    }

    /// <summary>
    /// �ڵ����� �κ� ������ ����
    /// </summary>
    public async void QuickJoin()
    {
        try
        {
            // ���� Ȱ��ȭ�� �κ� �� �ϳ��� ������ ������Ŵ
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            string relayCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value; // ������ �κ� �����Ϳ��� Relay ������ ������ �� ����� join �ڵ�
            JoinAllocation joinAllocation = await JoinRelay(relayCode); // join �ڵ带 ����Ͽ� Relay �Ҵ翡 ����

            // NetworkManager�� UnityTransport ���� ��ҿ� Relay ���� ������ ����
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            // Ŭ���̾�Ʈ�μ� ���� ������ ����
            NetworkGameManager.instance.StartClient();
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex); // ���� �޽��� ���
            LobbyBrowseUI.instance.LobbyConnectError(ex.Reason.ToString()); // �κ� ���� ���� �޽��� UI�� ����
        }
    }

    /// <summary>
    /// Ư�� �κ� �ڵ带 ����Ͽ� �κ� ����
    /// </summary>
    /// <param name="lobbyCode"></param>
    public async void JoinByCode(string lobbyCode)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(lobbyCode)) // �κ� �ڵ尡 ��� �ְų� ������ ���
            {
                Debug.LogError("Lobby code cannot be empty or contain white space.");
                LobbyBrowseUI.instance.LobbyConnectError("Lobby code cannot be empty");
                return;
            }

            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode); // �κ� �ڵ�� �κ� ����

            string relayCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value; // ������ �κ� �����Ϳ��� Relay ������ ������ �� ����� join �ڵ� ��������
            JoinAllocation joinAllocation = await JoinRelay(relayCode); // join �ڵ带 ����Ͽ� Relay �Ҵ翡 ����

            // NetworkManager�� UnityTransport ���� ��ҿ� Relay ���� ������ ����
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            // Ŭ���̾�Ʈ�μ� ���� ������ ����
            NetworkGameManager.instance.StartClient();
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex); // ���� �޽��� ���
            LobbyBrowseUI.instance.LobbyConnectError(ex.Reason.ToString()); // �κ� ���� ���� UI�� ����
        }
    }

    /// <summary>
    /// Ư�� �κ� ID�� ����Ͽ� �κ� ���� (�κ� ����Ʈ���� Ŭ���ؼ� ������ �� ���)
    /// </summary>
    /// <param name="lobbyID"></param>
    public async void JoinByID(string lobbyID)
    {
        try
        {
            // �κ� ID�� �κ� ����
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyID);

            // ������ �κ� �����Ϳ��� Relay ������ ������ �� ����� join �ڵ带 �����´�.
            string relayCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayCode); // join �ڵ带 ����Ͽ� Relay �Ҵ翡 ����

            // NetworkManager�� UnityTransport ���� ��ҿ� Relay ���� �����͸� ����
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            // Ŭ���̾�Ʈ�μ� ���� ������ ����
            NetworkGameManager.instance.StartClient();
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex); // ���� �޽��� ���
            LobbyBrowseUI.instance.LobbyConnectError(ex.Reason.ToString()); // �κ� ���� ���� UI�� ����
        }
    }

    /// <summary>
    /// ���� �κ� ������
    /// </summary>
    public async void LeaveLobby()
    {
        if (joinedLobby != null)
        {
            try
            {
                // �÷��̾� ��Ͽ��� ����, �ڱ� �ڽ� ID �Ӹ� �ƴ϶� �ٸ� �÷��̾��� ID�� ���� ����(���� ��� ���� �� ����)
                // �÷��̾ ���� ������ ���� �ִ� �÷��̾� �� �� ���� ȣ��Ʈ �÷��̾ ��
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                joinedLobby = null;
            }
            catch (LobbyServiceException ex)
            {
                Debug.Log(ex); // ���� �޽��� ���
            }
        }
    }

    /// <summary>
    /// ���� �κ� ����
    /// </summary>
    public async void DeleteLobby()
    {
        if (joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id); // �κ� ����
                joinedLobby = null;
            }
            catch (LobbyServiceException ex)
            {
                Debug.Log(ex);
            }
        }
    }

    /// <summary>
    /// �κ� ����� �����ϰ� �̺�Ʈ�� ���� �κ� ����� ����Ǿ����� �˸�
    /// </summary>
    async void ListLobbies()
    {
        try
        {
            // ���� �ɼ� ����
            QueryLobbiesOptions options = new QueryLobbiesOptions
            {
                Filters = new List<QueryFilter>
                {
                    // �̿� ������ ����(�÷��̾ ������ �� �ִ� ���� ����)�� 0���� ū �κ�鸸 ���͸�
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                },

                // ���� �ð� �������� �������� ����
                Order = new List<QueryOrder>()
                {
                    new QueryOrder(asc: false, field: QueryOrder.FieldOptions.Created)
                }
            };

            // �κ� ���񽺸� ����Ͽ� �κ� ������ �񵿱������� ����
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(options);
            
            // �κ� ����� ����Ǿ����� �̺�Ʈ�� ���� �˸�
            // �̺�Ʈ�� null�� �ƴ� ��쿡�� ȣ��ȴ�. ��, �̺�Ʈ �ڵ鷯�� ��ϵǾ� ���� ������ ȣ����� �ʴ´�.
            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs
            {
                lobbyList = queryResponse.Results // ���� ����� �κ� ���
            });
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
        }
    }

    private void Update()
    {
        LobbyHeartBeat();  // ��Ʈ��Ʈ Ÿ�̸� ����
        LobbyListUpdate(); // �κ� ��� ����
    }

    /// <summary>
    /// �κ� ȣ��Ʈ�� ��� �ֱ������� ��Ʈ��Ʈ ���� ���� �κ� ���� ����
    /// </summary> 
    void LobbyHeartBeat()
    {
        if (IsLobbyHost()) // ���� �κ� ���� ����ڰ� ȣ��Ʈ����
        {
            heartBeatTimer -= Time.deltaTime;
            if (heartBeatTimer <= 0)
            {
                heartBeatTimer = heartBeatTimerMax;
                LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id); // ������ �κ� Ȱ�� �������� �˸���. (�̰� ���ϰ� �� �ٿ� �����ǰ� ���� ����)
            }
        }
    }

    /// <summary>
    /// �κ� �˻� â�� ��� ������ �κ� �˻� â ������Ʈ
    /// </summary>
    void LobbyListUpdate()
    {
        if (joinedLobby == null && AuthenticationService.Instance.IsSignedIn &&
            SceneManager.GetSceneByName("MULTIPLAYER_LOBBY_BROWSE_SCENE") == SceneManager.GetActiveScene())
        {
            listLobbiesTimer -= Time.deltaTime;
            if (listLobbiesTimer <= 0)
            {
                listLobbiesTimer = listLobbiesTimerMax;
                ListLobbies();
            }
        }
    }

    /// <summary>
    /// ���� �÷��̾ �κ� ȣ��Ʈ���� Ȯ��
    /// </summary>
    /// <returns></returns>
    bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    /// <summary>
    /// ���� ������ �κ� ��ȯ
    /// </summary>
    /// <returns></returns>
    public Lobby GetJoinedLobby()
    {
        return joinedLobby;
    }
}
