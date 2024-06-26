using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// ���� �濡�� �÷��̾� ���� ǥ���ϰ� �����ϴ� Ŭ����
/// </summary>
public class LobbyPlayer : NetworkBehaviour
{
    //SkinSelector skinSelector;
    public int index;

    public TMP_Text usernameText;

    private void Start()
    {
        NetworkGameManager.onPlayerDataListChanged += UpdatePlayer; // �÷��̾� �����Ͱ� ����� ������ ȣ��
        //skinSelector = GetComponent<SkinSelector>();
        UpdatePlayer(); // ó���̴ϱ� �� ���� ������Ʈ
    }

    /// <summary>
    /// Ư�� �ε����� �ش��ϴ� �÷��̾ ����Ǿ� ������ �ش� �÷��̾��� �����͸� UI�� �ݿ��ϰ� ȭ�翡 ǥ��, ����Ǿ� ���� ������ ����
    /// </summary>
    void UpdatePlayer()
    {
        //Debug.Log("Update");
        if (NetworkGameManager.instance.IsPlayerIndexConnected(index))
        {
            Show();

            PlayerData data = NetworkGameManager.instance.GetPlayerDataFromIndex(index); // �ε����� �ش��ϴ� �÷��̾� ��������
            //skinSelector.UpdateSkin(NetworkGameManager.instance.GetPlayerSkinFromIndex(index));
            usernameText.text = data.username.ToString(); // �ͼ� TextMeshPro�� ����
        }
        else
        {
            Hide();
        }
    }

    /// <summary>
    /// �÷��̾ ȭ�鿡 ǥ��
    /// </summary>
    void Show()
    {
        //FindObjectOfType<CharacterSelectUI>().skinSelector = skinSelector;
        gameObject.SetActive(true);
        //skinSelector.ChangeSkinIndex(GameManager.instance.GetPlayerSkin(index));

    }

    /// <summary>
    /// �÷��̾ ȭ�鿡�� ����
    /// </summary>
    void Hide()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// ������Ʈ�� �ı��� ��, �̺�Ʈ ��� ����
    /// </summary>
    public override void OnDestroy()
    {
        NetworkGameManager.onPlayerDataListChanged -= UpdatePlayer;
    }
}
