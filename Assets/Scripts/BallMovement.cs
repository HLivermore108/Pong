using UnityEngine;

public class BallMovement : MonoBehaviour, ICollidable
{
    [SerializeField] private float speed = 5f; // private field
    [SerializeField] private Vector2 direction = new Vector2(1f, 1f); // private field

    private Rigidbody2D rb;
    private Vector2 lastVelocity;

    public float Speed
    {
        get => speed;
        set => speed = Mathf.Max(0f, value); // prevents negative speed
    }

    public Vector2 Direction
    {
        get => direction;
        set => direction = value.normalized; // keeps it normalized
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        direction = direction.normalized;
    }

    private void Start()
    {
        rb.linearVelocity = direction * speed;
    }

    private void FixedUpdate()
    {
        // Cache velocity before collision resolution
        lastVelocity = rb.linearVelocity;

        // Keeps constant speed over time
        if (rb.linearVelocity.sqrMagnitude > 0.0001f)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * speed;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        ICollidable collidable =
            collision.gameObject.GetComponent<ICollidable>();

        if (collidable != null)
        {
            collidable.OnHit(collision);
        }
    }

    public void OnHit(Collision2D collision)
    {
        // Wall bounce Y flip
        if (collision.gameObject.CompareTag("TopWall") ||
            collision.gameObject.CompareTag("BottomWall"))
        {
            rb.linearVelocity = new Vector2(
                lastVelocity.x,
                -lastVelocity.y
            ).normalized * Speed;

            return;
        }

        // Paddle bounce X flip
        if (collision.gameObject.GetComponent<PaddleController>() != null)
        {
            rb.linearVelocity = new Vector2(
                -lastVelocity.x,
                lastVelocity.y
            ).normalized * Speed;
        }
    }

}
