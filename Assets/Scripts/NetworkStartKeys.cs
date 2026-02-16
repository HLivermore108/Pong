using Unity.Netcode;
using UnityEngine;

public class NetworkStartKeys : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H)) NetworkManager.Singleton.StartHost();
        if (Input.GetKeyDown(KeyCode.C)) NetworkManager.Singleton.StartClient();
        if (Input.GetKeyDown(KeyCode.H) && !NetworkManager.Singleton.IsListening)
            NetworkManager.Singleton.StartHost();

    }
}
