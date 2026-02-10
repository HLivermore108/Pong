using UnityEngine;

public abstract class PaddleController : NetworkedObject, ICollidable
{
    [SerializeField] protected float moveSpeed = 8f;
    protected Rigidbody2D rb;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    protected abstract float GetMovementInput();

    protected virtual void FixedUpdate()
    {
        float input = GetMovementInput();
        rb.linearVelocity = new Vector2(0f, input * moveSpeed);
    }

    public virtual void OnHit(Collision2D collision)
    {
        BallMovement ball = collision.otherCollider.GetComponent<BallMovement>();
        if (ball == null) return;

        ball.BounceHorizontal(collision);
        ball.ToggleColor();
    }

    public override void Initialize()
    {

    }

    public override int GetNetworkId()
    {
        return gameObject.GetInstanceID();
    }

}
