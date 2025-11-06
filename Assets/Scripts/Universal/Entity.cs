using UnityEngine;

public class Entity : MonoBehaviour
{
    // Lock states
    public bool movementLocked = false;
    public bool attackLocked = false;

    public bool isAlive = true;
    public bool isFacingRight = true;
    // Other variables like move speed, jump height, etc.

    // References to components
    [Header("Component References")]
    public Rigidbody2D rb;
    public Animator animator;

    // Flip the character to face the direction of movement
    public void FlipOnVelocityX()
    {
        if (rb.linearVelocityX < 0 && isFacingRight)
            Flip();
        else if (rb.linearVelocityX > 0 && !isFacingRight)
            Flip();
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
