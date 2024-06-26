using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LobbyJoinedUI : MonoBehaviour
{
    public GameObject readyButton; // �غ� ��ư
   // public GameObject[] arrows;

    public GameObject unreadyButton; // �غ����� ��ư

    public TMP_Text lobbyNameText; // �κ� �̸� ǥ�õǴ� ��
    public TMP_Text lobbyCodeText; // �κ� �ڵ� ǥ�õǴ� ��

    public GameObject leaveLobbyButton; // �κ� ������ ��ư

    private void Awake()
    {
        readyButton.SetActive(true); // �غ� ��ư Ȱ��ȭ

        //arrows[0].SetActive(true);
        //arrows[1].SetActive(true);
    }

    private void Start()
    {
        Lobby lobby = LobbyManager.instance.GetJoinedLobby();  // ���� �������� �κ� ��������
        lobbyNameText.text = "Lobby: " + lobby.Name;           // �κ� �̸� ǥ��
        lobbyCodeText.text = "Lobby Code: " + lobby.LobbyCode; // �κ� �ڵ� ǥ��

        //NetworkGameManager.instance.ChangePlayerSkin(Random.Range(0, 111));
    }
    public void ReadyPressed() // �غ� ��ư ������ �� ȣ��Ǵ� �޼���
    {
        readyButton.SetActive(false);
        unreadyButton.SetActive(true);

        leaveLobbyButton.SetActive(false);
    }

    public void UnReadyPressed() // �غ� ���� ��ư ������ �� ȣ��Ǵ� �޼���
    {
        unreadyButton.SetActive(false);
        readyButton.SetActive(true);

        leaveLobbyButton.SetActive(true);

    }

    public void arrowPressed(int amount) // ���� ���� �޼��� (�÷��̾� ��Ų �ٲٴ°�)
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

    public void LeaveLobbyPressed() // �κ� ������ ��ư�� ������ �� ȣ��Ǵ� �޼���
    {
        LobbyManager.instance.LeaveLobby();  // �κ� ������
        NetworkManager.Singleton.Shutdown(); // ��Ʈ��ũ ���� ����
        SceneManager.LoadScene("MAINMENU_SCENE");
    }
}
