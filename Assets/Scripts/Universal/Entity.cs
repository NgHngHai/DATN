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
    public AnimationStateMachine animStateMachine;

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

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
