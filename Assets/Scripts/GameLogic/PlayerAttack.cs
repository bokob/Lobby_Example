using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAttack : NetworkBehaviour
{
    bool isAttack;

    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (IsLocalPlayer)
            Attack();
    }

    void Attack()
    {
        isAttack = Input.GetMouseButtonDown(0);
        if (isAttack)
            animator.SetTrigger("isAttack");
    }

    IEnumerator Attacked()
    {
        animator.SetTrigger("isAttacked");
        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
            StartCoroutine(Attacked());
    }
}
