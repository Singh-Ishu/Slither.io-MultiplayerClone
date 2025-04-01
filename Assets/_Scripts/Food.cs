using Unity.Netcode;
using UnityEngine;

public class Food : NetworkBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;

        if (!NetworkManager.Singleton.IsServer) return;

        if(col.TryGetComponent(out PlayerLength playerLength))
        {
            playerLength.AddLength();
        }
        else if(col.TryGetComponent(out Tail tail))
        {
            tail.networkedOwner.GetComponent<PlayerLength>().AddLength();
        }

        NetworkObject.Despawn();
        Debug.Log("food was destroyed");
    }
}
