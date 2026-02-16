using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(BallMovement))]
public class NetworkBallController : NetworkBehaviour
{
    private BallMovement ball;

    private void Awake()
    {
        ball = GetComponent<BallMovement>();
    }

    public override void OnNetworkSpawn()
    {
        if (ball != null)
            ball.enabled = IsServer;
    }
}
