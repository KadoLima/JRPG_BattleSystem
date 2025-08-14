using Unity.Netcode;
using UnityEngine;

public class OnlineTestButtons : MonoBehaviour
{
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartHost();
        }
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            StartClient();
        }
    }

    private void StartHost()
    {
        Debug.LogWarning("Starting Host...");
        NetworkManager.Singleton.StartHost();
    }

    private void StartClient()
    {
        Debug.LogWarning("Starting Client...");
        NetworkManager.Singleton.StartClient();
    }
}
