using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody2D))]
public class NetworkPaddleController : NetworkBehaviour, ICollidable
{
    [SerializeField] private float moveSpeed = 8f;

    [Header("Input Axes (Old Input Manager)")]
    [SerializeField] private string leftAxis = "LeftPaddle";
    [SerializeField] private string rightAxis = "RightPaddle";

    [Header("Spawn Positions")]
    [SerializeField] private float leftX = -7f;
    [SerializeField] private float rightX = 7f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            bool isLeftPlayer = (OwnerClientId == 0);
            float x = isLeftPlayer ? leftX : rightX;
            transform.position = new Vector3(x, 0f, transform.position.z);
        }
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        string axis = (OwnerClientId == 0) ? leftAxis : rightAxis;
        float input = Input.GetAxis(axis);

        SubmitInputServerRpc(input);
    }

    [ServerRpc]
    private void SubmitInputServerRpc(float input)
    {
        rb.linearVelocity = new Vector2(0f, input * moveSpeed);
    }

    public void OnHit(Collision2D collision)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        BallMovement ball = collision.otherCollider.GetComponent<BallMovement>();
        if (ball == null) return;

        ball.BounceHorizontal(collision);
        var colorSync = ball.GetComponent<NetworkBallColor>();
        if (colorSync != null) colorSync.Toggle();
    }
}
