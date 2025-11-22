using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour
{
    // Lock states
    public bool movementLocked;
    public bool attackLocked = false;

    public bool isAlive = true;
    public bool isFacingRight = true;

    public bool canBeKnockedback = true;

    // References to components
    [Header("Component References")]
    public Rigidbody2D rb;
    public Animator animator;
    public AnimationStateMachine animStateMachine;

    private Coroutine _movementLockRoutine;

    private void Awake()
    {
        animStateMachine = new AnimationStateMachine(); // Tạo FSM riêng cho entity
        animator = GetComponent<Animator>();            // Lấy component Animator
        rb = GetComponent<Rigidbody2D>();               // Lấy component Rigidbody2D
    }

    private void Update()
    {
        animStateMachine.UpdateCurrentState();          // Cập nhật state hiện tại mỗi frame
    }

    // Gọi từ Animation Event cuối clip (Attack/Hurt)
    // Mục đích: báo rằng animation này đã chạy xong
    public void CallCurrentAnimationStateTrigger()
    {
        animStateMachine.currentState?.CallAnimationTrigger();
    }

    // Flip the character to face the direction of movement
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

    public int FacingDir => isFacingRight ? 1 : -1;

    /// Apply knockback force to this entity. Optionally lock movement for a duration.
    public virtual void ApplyKnockback(Vector2 direction, float force, bool lockMovement, float lockDuration = 0f)
    {
        if (!canBeKnockedback) return;

        if (rb == null) return;

        if (direction.sqrMagnitude < 0.0001f)
            direction = new Vector2(-FacingDir, 0f);
        else
            direction.Normalize();


        if (lockMovement && lockDuration > 0f)
        {
            LockMovementFor(lockDuration);
        }

        rb.AddForce(direction * force, ForceMode2D.Impulse);
    }

    /// Lock movement for a fixed duration (overwrites any previous lock timer).
    public void LockMovementFor(float duration)
    {
        if (_movementLockRoutine != null)
            StopCoroutine(_movementLockRoutine);

        movementLocked = true;
        _movementLockRoutine = StartCoroutine(UnlockMovementAfter(duration));
    }

    private IEnumerator UnlockMovementAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        movementLocked = false;
        _movementLockRoutine = null;
    }
}
