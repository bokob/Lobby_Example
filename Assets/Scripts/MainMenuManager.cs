using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// MULTIPLAYER_LOBBY_BROWSE_SCENE 으로 전환하는 메소드를 제공하는 클래스
/// </summary>
public class MainMenuManager : MonoBehaviour
{
   public void LoadLobbyBrowseScene() 
    {
        SceneManager.LoadScene("MULTIPLAYER_LOBBY_BROWSE_SCENE");
    }
}
