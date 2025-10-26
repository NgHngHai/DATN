using UnityEngine;

/// <summary>
/// Base class for all enemies.
/// Handles movement, facing direction, and state machine updates.
/// </summary>
public abstract class Enemy : MonoBehaviour
{
    public float moveSpeed;
    public EnemyStateMachine logicStateMachine;

    protected Rigidbody2D rb;
    protected bool isFacingRight = true;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        logicStateMachine = new EnemyStateMachine();
    }

    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
        logicStateMachine.UpdateCurrentState();
    }

    public virtual void SetVelocity(Vector2 velocity)
    {
        rb.linearVelocity = velocity;
    }

    public void StopVelocity()
    {
        rb.linearVelocity = Vector2.zero;
    }

    public void FlipOnVelocityX()
    {
        if (rb.linearVelocityX < 0 && isFacingRight)
            Flip();
        else if (rb.linearVelocityX > 0 && !isFacingRight)
            Flip();
    }

    public void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public int facingDir => isFacingRight ? 1 : -1;
}
