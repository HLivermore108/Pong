using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PaddleController : MonoBehaviour
{
    [SerializeField] protected float moveSpeed = 8f;

    protected Rigidbody2D rb;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    protected virtual string GetAxisName()
    {
        return "Vertical"; // fallback default
    }

    protected virtual void FixedUpdate()
    {
        float input = Input.GetAxis(GetAxisName()); // -1..1
        rb.linearVelocity = new Vector2(0f, input * moveSpeed);
    }
}
