using UnityEngine;
using Unity.Netcode;  // Required for multiplayer

public class NetworkManagerHandler : MonoBehaviour
{
    void Awake()
    {
        if (FindObjectsOfType<NetworkManager>().Length > 1)
        {
            Destroy(gameObject);  // Prevent duplicate instances
            return;
        }

        DontDestroyOnLoad(gameObject);  // Keep NetworkManager across scenes
    }
}
