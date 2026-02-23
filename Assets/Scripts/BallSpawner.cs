using UnityEngine;
using Unity.Netcode;

public class BallSpawner : MonoBehaviour
{
    [SerializeField] private NetworkObject ballPrefab;

    private NetworkObject spawnedBall;
    private bool hasSpawned = false;

    private void Update()
    {
        if (hasSpawned) return;
        if (NetworkManager.Singleton == null) return;
        if (!NetworkManager.Singleton.IsServer) return;
        if (!NetworkManager.Singleton.IsListening) return;

        SpawnBall();
    }

    private void SpawnBall()
    {
        hasSpawned = true;

        spawnedBall = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);
        spawnedBall.Spawn();

        // ✅ Tell GameManager which ball to control
        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            BallMovement bm = spawnedBall.GetComponent<BallMovement>();
            gm.SetBallReference(bm);
        }

        Debug.Log("Server spawned ball and assigned it to GameManager.");
    }
}