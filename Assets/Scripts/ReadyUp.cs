using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class ReadyUp : NetworkBehaviour
{
    LobbyJoinedUI ui;
    Dictionary<ulong, bool> playerReadyDictionary; // �� Ŭ���̾�Ʈ�� �غ� ���¸� �����ϴ� ��ųʸ�

    public bool isLocalPlayerReady = false;        // ���� �÷��̾�(�ڱ� �ڽ�)�� �غ� ���¸� ��Ÿ����. 
    // Start is called before the first frame update
    private void Awake()
    {
        playerReadyDictionary = new Dictionary<ulong, bool>();
        ui = GetComponent<LobbyJoinedUI>();
    }
    public void SetLocalPlayerReady()
    {
        isLocalPlayerReady = true;
        SetPlayerReadyServerRpc(NetworkGameManager.instance.GetPlayerDataIndexFromClientID(NetworkManager.Singleton.LocalClientId));
        //Debug.Log(NetworkManager.Singleton.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    void SetPlayerReadyServerRpc(int _index, ServerRpcParams rpcParams = default) // �������� ȣ��Ǵ� RPC �޼���
    {
        // ServerRpcParams�� RPC�� ȣ���� Ŭ���̾�Ʈ�κ��� ���޵� ������ ��� �ִ� �Ӽ�

        playerReadyDictionary[rpcParams.Receive.SenderClientId] = true; // Rpc�� ȣ���� Ŭ���̾�Ʈ�� �غ� ���¸� true�� ����

        SetPlayerReadyDisplayClientRpc(_index); // �ٸ� Ŭ���̾�Ʈ���� �����ֱ� ���� �������� �����Ѵ�. �׷��� �ٸ� Ŭ���̾�Ʈ���� �� RPC ȣ���Ѵ�.

        bool allClientsReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                //this player not ready
                allClientsReady = false;
                break;
            }
        }
        Debug.Log("All clients ready: " + allClientsReady);
        if (allClientsReady == true) // ��� Ŭ���̾�Ʈ�� �غ� ���¸�, ���Ӿ� �ε�
        {
            NetworkGameManager.instance.LoadGameScene();
        }
    }

    [ClientRpc]
    void SetPlayerReadyDisplayClientRpc(int _index)
    {
        //GameManager.instance.GetPlayerDataFromIndex(_index);
        LobbyPlayer[] _players = FindObjectsOfType<LobbyPlayer>();
        foreach (LobbyPlayer player in _players)
        {
            if (player.index == _index)
            {
                player.usernameText.color = Color.green;
                break;
            }
        }
    }

    public void SetLocalPlayerUnready()
    {
        isLocalPlayerReady = false;
        SetPlayerUnreadyServerRpc(NetworkGameManager.instance.GetPlayerDataIndexFromClientID(NetworkManager.Singleton.LocalClientId));
    }

    [ServerRpc(RequireOwnership = false)]
    void SetPlayerUnreadyServerRpc(int _index, ServerRpcParams rpcParams = default) // �������� �����ϴ� RPC �޼���
    {
        playerReadyDictionary[rpcParams.Receive.SenderClientId] = false;
        SetPlayerUnreadyDisplayClientRpc(_index); // �ٸ� Ŭ���̾�Ʈ�� �ݿ��� �� �ֵ��� Ŭ���̾�Ʈ RPC ȣ��
    }
    [ClientRpc]
    void SetPlayerUnreadyDisplayClientRpc(int _index) // Ư�� �÷��̾��� �غ� ��� ���¸� UI�� ǥ��
    {
        LobbyPlayer[] _players = FindObjectsOfType<LobbyPlayer>();
        foreach (LobbyPlayer player in _players)
        {
            if (player.index == _index)
            {
                player.usernameText.color = Color.white;
                break;
            }
        }
    }
}
