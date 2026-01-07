using UnityEngine;

public class ConstantBounce2D : MonoBehaviour
{
    public float bounceSpeed = 30f; // choose the speed you want

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.rigidbody;
        if (rb == null) return;

        // Get normal of contact (direction outward)
        Vector2 normal = collision.GetContact(0).normal;

        // Reverse it (point outward)
        Vector2 bounceDir = -normal.normalized;

        // Force set the ball's velocity
        rb.linearVelocity = bounceDir * bounceSpeed;
        rb.AddForce(bounceDir * 5f, ForceMode2D.Impulse);
    }
}