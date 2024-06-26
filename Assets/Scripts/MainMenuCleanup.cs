using Unity.Netcode;
using UnityEngine;

/// <summary>
/// 남아 있는 매니저들을 모두 없애는 클래스
/// </summary>
public class MainMenuCleanup : MonoBehaviour
{
    void Awake()
    {
        // 이전에 PlayOnline 버튼을 누르고 다른 씬으로 전환했다가 MAINMENU_SCENE으로 돌아온거면 원래 상태로 만들기 위해 만들어졌던 매니저들을 없앤다.

        if (NetworkManager.Singleton != null)
            Destroy(NetworkManager.Singleton.gameObject);
        if (NetworkGameManager.instance != null)
            Destroy(NetworkGameManager.instance.gameObject);
        if (LobbyManager.instance != null)
            Destroy(LobbyManager.instance.gameObject);
    }
}