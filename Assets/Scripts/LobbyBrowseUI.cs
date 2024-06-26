using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// �κ� �˻� UI
/// </summary>
public class LobbyBrowseUI : MonoBehaviour
{
    public GameObject mainPanel;

    public TMP_InputField lobbyNameInput;            // �κ� �̸� �Է�
    public Toggle isPrivate;                         // ��� �� ������� ����

    public TMP_InputField joinCodeInput;             // ���� �ڵ� �Է�â
    public TMP_InputField usernameInput;             // ���� �̸� �Է�â

    public GameObject lobbyContainer;                // �κ� ��ϵ� ǥ���� ��
    public GameObject lobbyListTemplate;             // �κ� (�κ� ��Ͽ� ǥ�õǴ� ���� ��)

    public GameObject connectionResponseUI;          // '������' ��Ÿ���� UI
    public TMP_Text messsageText;                    // ���� ���� ����
    public GameObject connectionResponseCloseButton; // ���� ���� ���� �� �ݴ� ��ư

    public static LobbyBrowseUI instance;

    private void Awake()
    {
        instance = this;                    // �̱��� �����

        lobbyListTemplate.SetActive(false); // �κ� ���ø� ��Ȱ��ȭ, ���� �κ� ������ �� �����Ͽ� ���
        lobbyContainer.SetActive(true);     // �κ� �����̳� ����, �κ���� ǥ�õ�
    }

    private void Start()
    {
        usernameInput.text = NetworkGameManager.instance.GetUsername(); // ����ڸ� �Է� �ʵ� �ʱⰪ ����
        usernameInput.onValueChanged.AddListener((string newText) =>
        {
            NetworkGameManager.instance.SetUsername(newText);   // ����� �� ���� �� NetworkGameManager�� ���ο� ����ڸ� ����
        });

        LobbyManager.instance.OnLobbyListChanged += GameLobby_OnLobbyListChanged; // �̺�Ʈ ���, �κ� ����� ����� ������ ȣ��
    }

    private void GameLobby_OnLobbyListChanged(object sender, LobbyManager.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }
    public void CreateLobbyPressed() // �κ� ���� ��ư ������ �� ȣ��Ǵ� �޼���
    {
        //NetworkManager.Singleton.StartHost();
        mainPanel.SetActive(false);
        connectionResponseUI.SetActive(true);
        string _lobby = "Lobby";
        messsageText.text = "Connecting...";

        if (lobbyNameInput.text == "" || lobbyNameInput.text == null)
        {
            _lobby = ("Lobby " + UnityEngine.Random.Range(100, 1000).ToString());
        }
        else
        {
            _lobby = lobbyNameInput.text;
        }
        LobbyManager.instance.CreateLobby(_lobby, isPrivate.isOn);
        //LoadCharacterSelectScene();
    }
    public void QuickJoinPressed() // ���� ���� ������ �� ȣ��Ǵ� �޼���
    {
        mainPanel.SetActive(false);
        lobbyContainer.SetActive(false);
        connectionResponseUI.SetActive(true);
        messsageText.text = "Connecting...";

        LobbyManager.instance.QuickJoin();
        //NetworkManager.Singleton.StartClient();
    }
    public void JoinCodePressed() // join code �Է��ϰ� ��ư ������ �� ȣ��Ǵ� �޼���
    {
        mainPanel.SetActive(false);
        lobbyContainer.SetActive(false);
        connectionResponseUI.SetActive(true);
        messsageText.text = "Connecting...";

        LobbyManager.instance.JoinByCode(joinCodeInput.text);
    }

    public void JoinLobbyById(string _lobbyId) // �κ� ������ �� ȣ��Ǵ� �޼���
    {
        mainPanel.SetActive(false);
        lobbyContainer.SetActive(false);
        connectionResponseUI.SetActive(true);
        messsageText.text = "Connecting...";

        LobbyManager.instance.JoinByID(_lobbyId);
    }

    public void UpdateLobbyList(List<Lobby> lobbyList) // �κ� ����� ����� �� ȣ��Ǿ� UI ������Ʈ
    {
        foreach (Transform child in lobbyContainer.transform)
        {
            if (child == lobbyListTemplate) continue;
            Destroy(child.gameObject); // ���� �κ� ��� UI ��� ����
        }
        foreach (Lobby lobby in lobbyList)
        {
            GameObject _lobby = Instantiate(lobbyListTemplate, lobbyContainer.transform); // �κ� ��� ���ø� �����ؼ� ���� �κ� UI ����
            _lobby.SetActive(true);
            _lobby.GetComponent<LobbyListItemUI>().SetLobby(lobby); // �� �κ� UI�� ���� �κ� ������ ����
        }
    }

    public void ConnectionFailed() // �κ� ���� ���нÿ� ���� ������ �ϴ� �޼���
    {
        messsageText.text = NetworkManager.Singleton.DisconnectReason.ToString();
        connectionResponseCloseButton.SetActive(true);
    }
    public void LobbyConnectError(string reason) // �κ� ���� ���� �޽��� ������ �ϴ� �޼���
    {
        messsageText.text = reason;
        connectionResponseCloseButton.SetActive(true);
    }
    public void CloseConnectionResponseUI() // ���� ���� �� �ݱ� ��ư ������ ȣ��Ǵ� �޼���
    {
        connectionResponseUI.SetActive(false);
        connectionResponseCloseButton.SetActive(false);
        mainPanel.SetActive(true);
    }

    /// <summary>
    /// ���� �޴��� ���ư���
    /// </summary>
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// ������Ʈ�� �ı��� ��, �̺�Ʈ ��� ����
    /// </summary>
    private void OnDestroy()
    {
        LobbyManager.instance.OnLobbyListChanged -= GameLobby_OnLobbyListChanged;
    }
}
