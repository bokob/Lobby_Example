using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] Transform cam;

    [SerializeField] Transform player;

    Animator animator;

    [SerializeField] float moveSpeed = 3f;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if(IsLocalPlayer) // �ڱ� �ڽŸ� ������ �� �ְ�, �̰� �� �ϸ� �ٸ� ��� Ű �Էµ� �ν��ϴ� ���� ����
            Move();
    }

    private void Move()
    {
        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool isMove = moveInput.magnitude != 0;

        animator.SetBool("isMove", isMove);
        if (isMove)
        {
            Vector3 lookForward = new Vector3(cam.forward.x, 0f, cam.forward.z).normalized;
            Vector3 lookRight = new Vector3(cam.right.x, 0f, cam.right.z).normalized;
            Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;

            player.forward = moveDir;
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }

        Debug.DrawRay(cam.position, new Vector3(cam.forward.x, 0f, cam.forward.z).normalized, Color.red);
    }
}
