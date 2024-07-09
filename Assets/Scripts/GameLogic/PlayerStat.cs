using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerStat : NetworkBehaviour
{
    public NetworkVariable<int> hp = new NetworkVariable<int>();

    Animator animator;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        hp.Value = 100;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        Debug.Log(NetworkGameManager.instance.GetUsernameFromClientId(OwnerClientId) + "가 맞음");

        Debug.Log(other.name + "이(가) 닿았다!");
        
        if (other.gameObject.CompareTag("Punch") && transform.root.GetComponent<NetworkObject>().OwnerClientId != other.transform.root.GetComponent<NetworkObject>().OwnerClientId)
        {
            GetComponent<PlayerStat>().hp.Value -= 50;
            PlayAttackedAnimationClientRpc();
        }
    }


    IEnumerator Attacked()
    {
        animator.SetTrigger("isAttacked");
        yield return null;
    }

    [ClientRpc]
    void PlayAttackedAnimationClientRpc()
    {
        StartCoroutine(Attacked());
    }
}
