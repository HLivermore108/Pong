using UnityEngine;

public class WallBounceVertical : MonoBehaviour, ICollidable
{
    public void OnHit(Collision2D collision)
    {
        // otherCollider is the ball's collider in this setup
        BallMovement ball = collision.otherCollider.GetComponent<BallMovement>();
        if (ball == null) return;

        ball.BounceVertical(collision);
        Debug.Log("Wall OnHit otherCollider: " + collision.otherCollider.name);

    }
}
