using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LobbyJoinedUI : MonoBehaviour
{
    public GameObject readyButton; // 준비 버튼
   // public GameObject[] arrows;

    public GameObject unreadyButton; // 준비해제 버튼

    public TMP_Text lobbyNameText; // 로비 이름 표시되는 곳
    public TMP_Text lobbyCodeText; // 로비 코드 표시되는 곳

    public GameObject leaveLobbyButton; // 로비 떠나기 버튼

    private void Awake()
    {
        readyButton.SetActive(true); // 준비 버튼 활성화

        //arrows[0].SetActive(true);
        //arrows[1].SetActive(true);
    }

    private void Start()
    {
        Lobby lobby = LobbyManager.instance.GetJoinedLobby();  // 현재 참여중인 로비 가져오기
        lobbyNameText.text = "Lobby: " + lobby.Name;           // 로비 이름 표시
        lobbyCodeText.text = "Lobby Code: " + lobby.LobbyCode; // 로비 코드 표시

        //NetworkGameManager.instance.ChangePlayerSkin(Random.Range(0, 111));
    }
    public void ReadyPressed() // 준비 버튼 눌렀을 때 호출되는 메서드
    {
        readyButton.SetActive(false);
        unreadyButton.SetActive(true);

        leaveLobbyButton.SetActive(false);
    }

    public void UnReadyPressed() // 준비 해제 버튼 눌렀을 때 호출되는 메서드
    {
        unreadyButton.SetActive(false);
        readyButton.SetActive(true);

        leaveLobbyButton.SetActive(true);

    }

    public void arrowPressed(int amount) // 쓸모 없는 메서드 (플레이어 스킨 바꾸는거)
    {
        int id = NetworkGameManager.instance.GetPlayerDataIndexFromClientID(NetworkManager.Singleton.LocalClientId);
        int index = NetworkGameManager.instance.GetPlayerSkinFromIndex(id);
        index += amount;
        if (index > 111)
            index = 0;
        if (index < 0)
            index = 111;
        NetworkGameManager.instance.ChangePlayerSkin(index);
    }

    public void LeaveLobbyPressed() // 로비 떠나기 버튼을 눌렀을 때 호출되는 메서드
    {
        LobbyManager.instance.LeaveLobby();  // 로비 떠나기
        NetworkManager.Singleton.Shutdown(); // 네트워크 세션 종료
        SceneManager.LoadScene("MAINMENU_SCENE");
    }
}
