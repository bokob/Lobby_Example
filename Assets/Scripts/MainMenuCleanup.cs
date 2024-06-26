using Unity.Netcode;
using UnityEngine;

/// <summary>
/// ���� �ִ� �Ŵ������� ��� ���ִ� Ŭ����
/// </summary>
public class MainMenuCleanup : MonoBehaviour
{
    void Awake()
    {
        // ������ PlayOnline ��ư�� ������ �ٸ� ������ ��ȯ�ߴٰ� MAINMENU_SCENE���� ���ƿ°Ÿ� ���� ���·� ����� ���� ��������� �Ŵ������� ���ش�.

        if (NetworkManager.Singleton != null)
            Destroy(NetworkManager.Singleton.gameObject);
        if (NetworkGameManager.instance != null)
            Destroy(NetworkGameManager.instance.gameObject);
        if (LobbyManager.instance != null)
            Destroy(LobbyManager.instance.gameObject);
    }
}