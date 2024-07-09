using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAttack : NetworkBehaviour
{
    bool isAttack;

    Animator animator;
    [SerializeField] SphereCollider punchCollider;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (IsLocalPlayer && Input.GetMouseButtonDown(0) && !isAttack)
            StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        isAttack = true;
        if (punchCollider && !punchCollider.enabled && isAttack)
        {
            punchCollider.enabled = true;
            animator.SetTrigger("isAttack");

            // 다른 클라이언트에서 punchCollider 활성화 되게 요청
            SetPunchColliderStateServerRpc(true);
        }

        yield return new WaitForSeconds(0.75f);

        punchCollider.enabled = false;
        isAttack = false;

        // 다시 비활성화
        SetPunchColliderStateServerRpc(false);
    }

    // punchCollider 상태를 서버에 요청하는 ServerRpc 메서드
    [ServerRpc]
    void SetPunchColliderStateServerRpc(bool state)
    {
        SetPunchColliderStateClientRpc(state);
    }

    // punchCollider 상태를 모든 클라이언트에서 설정하는 ClientRpc 메서드
    [ClientRpc]
    void SetPunchColliderStateClientRpc(bool state)
    {
        punchCollider.enabled = state;
    }
}
