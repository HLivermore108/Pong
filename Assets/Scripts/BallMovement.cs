using UnityEngine;

public class BallMovement : NetworkedObject, ICollidable
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private Vector2 direction = new Vector2(1f, 1f);

    [Header("Color Swap On Paddle Hit")]
    [SerializeField] private Color colorA = Color.blue;
    [SerializeField] private Color colorB = Color.red;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private Vector2 lastVelocity;
    private bool usingColorA = true;

    public float Speed
    {
        get => speed;
        set => speed = Mathf.Max(0f, value);
    }

    public Vector2 Direction
    {
        get => direction;
        set => direction = value.normalized;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        direction = direction.normalized;

        if (sr != null)
        {
            sr.color = colorA;
            usingColorA = true;
        }
    }

    void Start()
    {
        rb.linearVelocity = direction * speed;
    }

    void FixedUpdate()
    {
        lastVelocity = rb.linearVelocity;

        // Keep constant speed
        //if (rb.linearVelocity.sqrMagnitude > 0.0001f)
            // rb.linearVelocity = rb.linearVelocity.normalized * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Ball hit: " + collision.gameObject.name);

        // Ask the other object to respond if it implements ICollidable
        ICollidable collidable = collision.gameObject.GetComponent<ICollidable>();
        Debug.Log("Has ICollidable? " + (collidable != null));
        if (collidable != null)
        {
            collidable.OnHit(collision);
        }
    }

    // Required by interface
    public void OnHit(Collision2D collision) { }

    public void BounceVertical(Collision2D collision)
    {
        Vector2 incoming = lastVelocity;

        // If lastVelocity got flattened, use collision.relativeVelocity instead
        if (incoming.sqrMagnitude < 0.0001f || Mathf.Abs(incoming.y) < 0.0001f)
            incoming = collision.relativeVelocity;

        Vector2 bounced = new Vector2(incoming.x, -incoming.y);

        // Prevent a perfectly flat result
        if (Mathf.Abs(bounced.y) < 0.1f)
            bounced.y = Mathf.Sign(-incoming.y) * 0.1f;

        rb.linearVelocity = bounced.normalized * Speed;

        Debug.Log($"lastVelocity={lastVelocity}  relative={collision.relativeVelocity}");
    }

    public void BounceHorizontal(Collision2D collision)
    {
        Vector2 incoming = lastVelocity;

        if (incoming.sqrMagnitude < 0.0001f || Mathf.Abs(incoming.x) < 0.0001f)
            incoming = collision.relativeVelocity;

        Vector2 bounced = new Vector2(-incoming.x, incoming.y);

        if (Mathf.Abs(bounced.x) < 0.1f)
            bounced.x = Mathf.Sign(-incoming.x) * 0.1f;

        rb.linearVelocity = bounced.normalized * Speed;
    }


    public void ToggleColor()
    {
        if (sr == null) return;

        usingColorA = !usingColorA;
        sr.color = usingColorA ? colorA : colorB;
    }

    public override void Initialize()
    {

    }

    public override int GetNetworkId()
    {
        return gameObject.GetInstanceID();
    }

}
