using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(BallMovement))]
[RequireComponent(typeof(Rigidbody2D))]
public class NetworkBallController : NetworkBehaviour
{
    private BallMovement ball;
    private Rigidbody2D rb;

    private void Awake()
    {
        ball = GetComponent<BallMovement>();
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // Server simulates physics + runs BallMovement
            if (ball != null) ball.enabled = true;
            if (rb != null) rb.simulated = true;
        }
        else
        {
            // Clients do NOT simulate physics (prevents drift then snap-back)
            if (ball != null) ball.enabled = false;

            if (rb != null)
            {
                rb.simulated = false;
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }
    }
}