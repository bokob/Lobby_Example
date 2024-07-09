using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class HUDUI : NetworkBehaviour
{
    Camera cam;

    [SerializeField] TextMeshProUGUI playerName;
    NetworkVariable<FixedString128Bytes> networkPlayerName = new NetworkVariable<FixedString128Bytes> (
        "", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    void Start()
    {
        cam = Camera.main;
    }

    public override void OnNetworkSpawn()
    {
        // 닉네임 설정
        networkPlayerName.Value = NetworkGameManager.instance.GetUsernameFromClientId(OwnerClientId); // 클라이언트 ID로 플레이어 이름 가져옴
        playerName.text = networkPlayerName.Value.ToString();
    }

    void LateUpdate()
    {
        if(cam != null)
        {
            //Vector3 targetPosition = new Vector3(cam.transform.position.x, transform.position.y, cam.transform.position.z);
            //transform.LookAt(targetPosition);
            transform.LookAt(cam.transform);
            transform.rotation = Quaternion.LookRotation(cam.transform.forward);
        }
    }
}
