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

            // �ٸ� Ŭ���̾�Ʈ���� punchCollider Ȱ��ȭ �ǰ� ��û
            SetPunchColliderStateServerRpc(true);
        }

        yield return new WaitForSeconds(0.75f);

        punchCollider.enabled = false;
        isAttack = false;

        // �ٽ� ��Ȱ��ȭ
        SetPunchColliderStateServerRpc(false);
    }

    // punchCollider ���¸� ������ ��û�ϴ� ServerRpc �޼���
    [ServerRpc]
    void SetPunchColliderStateServerRpc(bool state)
    {
        SetPunchColliderStateClientRpc(state);
    }

    // punchCollider ���¸� ��� Ŭ���̾�Ʈ���� �����ϴ� ClientRpc �޼���
    [ClientRpc]
    void SetPunchColliderStateClientRpc(bool state)
    {
        punchCollider.enabled = state;
    }
}
