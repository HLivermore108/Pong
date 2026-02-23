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

    private void Awake()
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

    private void Start()
    {
        // Do not auto-launch. GameManager will launch on Start Game.
        EnsureRigidbody();
        rb.linearVelocity = Vector2.zero;
    }

    private void FixedUpdate()
    {
        // If this script is disabled on clients (server-authoritative), FixedUpdate won't run there.
        // Still: stay safe.
        if (!EnsureRigidbodySilent()) return;

        lastVelocity = rb.linearVelocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Ball hit: " + collision.gameObject.name);

        ICollidable collidable = collision.gameObject.GetComponent<ICollidable>();
        Debug.Log("Has ICollidable? " + (collidable != null));

        if (collidable != null)
        {
            collidable.OnHit(collision);
        }
    }

    // Required by interface (ball isn't using this directly in my pattern)
    public void OnHit(Collision2D collision) { }

    public void BounceVertical(Collision2D collision)
    {
        EnsureRigidbody();

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
        EnsureRigidbody();

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
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;

        usingColorA = !usingColorA;
        sr.color = usingColorA ? colorA : colorB;
    }

    public override void Initialize() { }

    public override int GetNetworkId()
    {
        return gameObject.GetInstanceID();
    }

    // Server-side helpers used by GameManager
    public void StopBallServer()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();

        // Stop physics motion
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        lastVelocity = Vector2.zero;

        // Reset position in a Rigidbody-friendly way
        rb.position = Vector2.zero;

        // If using NetworkTransform, teleport for immediate sync to clients
        var nt = GetComponent<Unity.Netcode.Components.NetworkTransform>();
        if (nt != null)
            nt.Teleport(Vector3.zero, transform.rotation, transform.localScale);
    }

    public void ResetAndLaunchServer(Vector2 launchDirection)
    {
        EnsureRigidbody();

        // Stop motion first
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        lastVelocity = Vector2.zero;

        // Reset position using Rigidbody
        rb.position = Vector2.zero;

        // Teleport for instant client sync (if using NetworkTransform)
        var nt = GetComponent<Unity.Netcode.Components.NetworkTransform>();
        if (nt != null)
            nt.Teleport(Vector3.zero, transform.rotation, transform.localScale);

        // Now launch
        Vector2 dir = launchDirection.normalized;
        rb.linearVelocity = dir * Speed;
    }

    // Helpers
    private void EnsureRigidbody()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();

        // If it's still null, my prefab is missing Rigidbody2D
        if (rb == null)
        {
            Debug.LogError("BallMovement is missing Rigidbody2D! Add Rigidbody2D to the Ball prefab.");
        }
    }

    private bool EnsureRigidbodySilent()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        return rb != null;
    }
}