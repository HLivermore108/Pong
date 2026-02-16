using UnityEngine;
using Unity.Netcode;

public class BallSpawner : MonoBehaviour
{
    [SerializeField] private NetworkObject ballPrefab;

    private NetworkObject spawnedBall;
    private bool hasSpawned = false;

    private void Update()
    {
        // Wait until networking is started and on the server
        if (hasSpawned) return;
        if (NetworkManager.Singleton == null) return;
        if (!NetworkManager.Singleton.IsServer) return;
        if (!NetworkManager.Singleton.IsListening) return;

        SpawnBall();
    }

    private void SpawnBall()
    {
        if (hasSpawned) return;
        hasSpawned = true;

        spawnedBall = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);
        spawnedBall.Spawn();
        Debug.Log("Server spawned ball.");
    }
}
