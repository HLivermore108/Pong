using UnityEngine;

public class WallBounceVertical : MonoBehaviour, ICollidable
{
    public void OnHit(Collision2D collision)
    {
        BallMovement ball = collision.otherCollider.GetComponent<BallMovement>();
        if (ball == null) return;

        ball.BounceVertical(collision);
    }
}
