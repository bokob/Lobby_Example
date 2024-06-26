using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// 게임 방에서 플레이어 정보 표시하고 관리하는 클래스
/// </summary>
public class LobbyPlayer : NetworkBehaviour
{
    //SkinSelector skinSelector;
    public int index;

    public TMP_Text usernameText;

    private void Start()
    {
        NetworkGameManager.onPlayerDataListChanged += UpdatePlayer; // 플레이어 데이터가 변경될 때마다 호출
        //skinSelector = GetComponent<SkinSelector>();
        UpdatePlayer(); // 처음이니까 한 번은 업데이트
    }

    /// <summary>
    /// 특정 인덱스에 해당하는 플레이어가 연결되어 있으면 해당 플레이어의 데이터를 UI에 반영하고 화며에 표시, 연결되어 있지 않으면 숨김
    /// </summary>
    void UpdatePlayer()
    {
        //Debug.Log("Update");
        if (NetworkGameManager.instance.IsPlayerIndexConnected(index))
        {
            Show();

            PlayerData data = NetworkGameManager.instance.GetPlayerDataFromIndex(index); // 인덱스에 해당하는 플레이어 가져오기
            //skinSelector.UpdateSkin(NetworkGameManager.instance.GetPlayerSkinFromIndex(index));
            usernameText.text = data.username.ToString(); // 와서 TextMeshPro에 기재
        }
        else
        {
            Hide();
        }
    }

    /// <summary>
    /// 플레이어를 화면에 표시
    /// </summary>
    void Show()
    {
        //FindObjectOfType<CharacterSelectUI>().skinSelector = skinSelector;
        gameObject.SetActive(true);
        //skinSelector.ChangeSkinIndex(GameManager.instance.GetPlayerSkin(index));

    }

    /// <summary>
    /// 플레이어를 화면에서 숨김
    /// </summary>
    void Hide()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 오브젝트가 파괴될 때, 이벤트 등록 해제
    /// </summary>
    public override void OnDestroy()
    {
        NetworkGameManager.onPlayerDataListChanged -= UpdatePlayer;
    }
}
