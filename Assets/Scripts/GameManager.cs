using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    /// <summary>
    /// ��Ʈ��ũ ��ü �ʱ�ȭ
    /// </summary>
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsHost) // ȣ��Ʈ�̸�
        {
            // �� �ε尡 �Ϸ�Ǿ��� ���� �̺�Ʈ�� 'SceneManager_OnLoadEventCompleted'�� ����Ѵ�.
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }

    /// <summary>
    /// Ư�� ���� �ε尡 �Ϸ�Ǿ��� �� ȣ��ȴ�.
    /// </summary>
    /// <param name="sceneName">�ε�� ���� �̸�</param>
    /// <param name="loadSceneMode">�� �ε� ���</param>
    /// <param name="clientsCompleted">�� �ε带 �Ϸ��� Ŭ���̾�Ʈ���� ID ���</param>
    /// <param name="clientsTimedOut">�� �ε带 �ð� �ʰ��� Ŭ���̾�Ʈ���� ID ���</param>
    private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        // server
        NetworkGameManager.instance.SpawnPlayers(); // ��� Ŭ���̾�Ʈ�� �� �Ϸ����� ��(�� Ŭ���̾�Ʈ�� �������� ��) �÷��̾ �����Ѵ�.

        if (IsServer) // �������� ó���� ����
        {
            // 
        }
    }
}
