using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    public float Yaxis;
    public float Xaxis;

    public Transform target;                    //Player

    [SerializeField] float rotSensitive = 3f;   //ī�޶� ȸ�� ����
    [SerializeField] float dis = 10f;           //ī�޶�� �÷��̾������ �Ÿ�
    [SerializeField] float smoothTime = 0.12f;  //ī�޶� ȸ���ϴµ� �ɸ��� �ð�
    [SerializeField] float RotationMax = 80f;   //ī�޶� ȸ������ �ִ�
    [SerializeField] float RotationMin = -10f;  //ī�޶� ȸ������ �ּ�

    private Vector3 targetRotation;
    private Vector3 currentVel;

    private void Start()
    {
        if(!IsLocalPlayer)
            gameObject.SetActive(false);
    }

    void LateUpdate()//Player�� �����̰� �� �� ī�޶� ���󰡾� �ϹǷ� LateUpdate
    {
        Yaxis = Yaxis + Input.GetAxis("Mouse X") * rotSensitive;    //���콺 �¿�������� �Է¹���, ī�޶��� Y�� ȸ���� ��� ����

        //Xaxis�� ���콺�� �Ʒ��� ������(�������� �Է� �޾�����) ���� �������� ī�޶� �Ʒ��� ȸ���Ѵ�
        Xaxis = Xaxis - Input.GetAxis("Mouse Y") * rotSensitive;    //���콺 ���Ͽ������� �Է¹���, ī�޶��� X�� ȸ���� ��� ���� 

        //X��ȸ���� �Ѱ�ġ�� �����ʰ� �������ش�.
        Xaxis = Mathf.Clamp(Xaxis, RotationMin, RotationMax);

        //SmoothDamp�� ���� �ε巯�� ī�޶� ȸ��
        targetRotation = Vector3.SmoothDamp(targetRotation, new Vector3(Xaxis, Yaxis), ref currentVel, smoothTime);
        this.transform.eulerAngles = targetRotation;

        //ī�޶��� ��ġ�� �÷��̾�� ������ ����ŭ �������ְ� ��� ����ȴ�.
        transform.position = target.position - transform.forward * dis;
    }
}
