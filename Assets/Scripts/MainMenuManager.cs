using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// MULTIPLAYER_LOBBY_BROWSE_SCENE ���� ��ȯ�ϴ� �޼ҵ带 �����ϴ� Ŭ����
/// </summary>
public class MainMenuManager : MonoBehaviour
{
   public void LoadLobbyBrowseScene() 
    {
        SceneManager.LoadScene("MULTIPLAYER_LOBBY_BROWSE_SCENE");
    }
}
