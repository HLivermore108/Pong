using UnityEngine;

public class PaddleMovement : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private string inputAxis = "LeftPaddle"; // set per paddle in Inspector

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float input = Input.GetAxis(inputAxis); // -1 to 1
        rb.linearVelocity = new Vector2(0f, input * speed);
    }
}
