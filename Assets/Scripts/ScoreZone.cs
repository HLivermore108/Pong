using Unity.Netcode;
using UnityEngine;

public class ScoreZone : NetworkBehaviour
{
    public enum ZoneType { LeftScoreZone, RightScoreZone }
    [SerializeField] private ZoneType zoneType;

    [SerializeField] private GameManager gameManager;

    private void Awake()
    {
        // auto-find
        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only server should award points
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer) return;

        // Make sure it's the ball
        BallMovement ball = other.GetComponent<BallMovement>();
        if (ball == null) return;

        // If ball went past LEFT paddle -> RIGHT scores
        if (zoneType == ZoneType.LeftScoreZone)
            gameManager.ScoreRight();
        else
            gameManager.ScoreLeft();
    }
}