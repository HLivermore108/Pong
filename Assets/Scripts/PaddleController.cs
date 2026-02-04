using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class PaddleController : MonoBehaviour, ICollidable
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

    }
}
