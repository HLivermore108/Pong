using Unity.VisualScripting;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    public float speed = 5f;
    
    private Rigidbody2D rb; //rb reference to Rigidbody2D component
    private Vector2 lastVelocity; // going to let us hold a velocity before it hits a wall since when it hits the wall it is 0 so if you reverse it's still 0.

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb.linearVelocity = new Vector2(1f, 1f).normalized * speed;
        
        /*Vector2 direction = new Vector2(1, 1).normalized; //normalized prevents accidental speed boosts
        rb.linearVelocity = direction * speed; // Applies movement */
    }

    private void FixedUpdate()
    {
        lastVelocity = rb.linearVelocity; // saves the velocity before unity resolves collision.
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Hit: " + collision.gameObject.name); //Print name of what ball hits

        //Check the tag of either TopWall or BottomWall.
        if (collision.gameObject.CompareTag("TopWall") || collision.gameObject.CompareTag("BottomWall")) 
        {
            rb.linearVelocity = new Vector2(lastVelocity.x, -lastVelocity.y);

            //rb.linearVelocity = new Vector2(rb.linearVelocity.x, -rb.linearVelocity.y);
            /*Vector2 velocity = rb.linearVelocity; // Copies current velocity into local variable. We do it because we can't safely modify part of rb.veloctiy directly. So we copy it, modify the copy, then assign it back.
            velocity.y *= -1; // This is the bounce. *= reverses y velocity.
            rb.linearVelocity = velocity; // applies modified velocity back to the Rigidbody. */
        }
    }
}
