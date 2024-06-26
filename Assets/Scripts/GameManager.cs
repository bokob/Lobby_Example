using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    /// <summary>
    /// 네트워크 객체 초기화
    /// </summary>
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsHost) // 호스트이면
        {
            // 씬 로드가 완료되었을 때의 이벤트에 'SceneManager_OnLoadEventCompleted'를 등록한다.
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }

    /// <summary>
    /// 특정 씬의 로드가 완료되었을 때 호출된다.
    /// </summary>
    /// <param name="sceneName">로드된 씬의 이름</param>
    /// <param name="loadSceneMode">씬 로드 모드</param>
    /// <param name="clientsCompleted">씬 로드를 완료한 클라이언트들의 ID 목록</param>
    /// <param name="clientsTimedOut">씬 로드를 시간 초과한 클라이언트들의 ID 목록</param>
    private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        // server
        NetworkGameManager.instance.SpawnPlayers(); // 모든 클라이언트가 씬 완료했을 때(각 클라이언트가 성공했을 때) 플레이어를 스폰한다.

        if (IsServer) // 서버에서 처리할 로직
        {
            // 
        }
    }
}
