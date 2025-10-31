using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Component references
    private Rigidbody2D rb;
    private Animator animator;
    public PlayerSkillManager skillManager;

    // Movement variables
    [Header("Movement Variables")]
    [Tooltip("When true, external systems own velocity. Player input should not write velocity.")]
    public bool movementLocked = false;
    public float moveSpeed = 7f;
    private float moveAmt;
    private bool isFacingRight = true;

    // Jumping
    [Header("Jump Variables")]
    public float jumpForce = 5f;
    public int maxExtraJumpCount = 1;
    private int extraJumpCount = 0;
    private bool isGrounded = false;
    // Coyote time
    public float coyoteTime = 0.1f;
    private float lastGroundedTime = -1f;
    // Advanced jumping
    public float jumpHoldTimeMax = 0.2f;     // Maximum time jump can be held
    public float jumpHoldForce = 50f;        // Extra force applied while holding jump
    public float originalGravityMultiplier = 3f; // Original gravity multiplier
    public float fallGravityMultiplier = 8f; // Gravity multiplier after peak
    public float maxFallSpeed = -50f;        // Maximum downward velocity
    private float jumpHoldTimer = 0f;
    private bool isJumping = false;

    // Attacking & skills
    [Header("Attack & skills Variables")]
    [Tooltip("When true, cannot input attacks.")]
    public bool attackLocked = false;
    public bool skillLocked = false;

    // Dashing
    [Header("Dash Variables")]
    public float dashSpeed = 25f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.5f;
    //public bool allowAirDash = true; // allow dashing while airborne
    private bool isDashing = false;
    private float dashTimeLeft = 0f;
    private float lastDashTime = -10f;

    // Input system
    [Header("Input Actions")]
    public InputActionAsset InputActions;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction dashAction;

    private InputAction attackAction;
    private InputAction hAttackAction;

    private InputAction healAction;
    private InputAction skillAction;

    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        //animator = GetComponent<Animator>();
        skillManager = GetComponent<PlayerSkillManager>();

        // Prefer actions from the assigned InputActionAsset/Player map (avoid InputSystem.actions global lookup).
        if (InputActions != null)
        {
            var map = InputActions.FindActionMap("Player");
            if (map != null)
            {
                moveAction = map.FindAction("Move");
                jumpAction = map.FindAction("Jump");
                dashAction = map.FindAction("Dash");

                attackAction = map.FindAction("Attack");
                hAttackAction = map.FindAction("Heavy Attack");

                healAction = map.FindAction("Heal");
                skillAction = map.FindAction("Skill");
                return;
            }
        }

        // Fallback to project-wide actions (only if asset/map not assigned)
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        dashAction = InputSystem.actions.FindAction("Dash");

        attackAction = InputSystem.actions.FindAction("Attack");
        hAttackAction = InputSystem.actions.FindAction("Heavy Attack");

        healAction = InputSystem.actions.FindAction("Heal");
        skillAction = InputSystem.actions.FindAction("Skill");
    }

    // Update is called once per frame
    void Update()
    {
        // Read movement input
        Vector2 moveVec = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;

        if (!isDashing && !movementLocked)
        {
            // Get horizontal movement amount
            moveAmt = moveVec.x;
            FlipDirection();
        }
        else
        {
            // Prevent stale input from writing velocities when locked
            moveAmt = 0f;
        }

        // dash input (uses a simple cooldown + allowAirDash check)
        bool canDash = !isDashing && !movementLocked && (Time.time - lastDashTime >= dashCooldown);
        if (dashAction != null && dashAction.WasPressedThisFrame() && canDash)
        {
            StartDash();
        }

        // Attack input - can be used while running (does not cancel horizontal control)
        if (attackAction != null && attackAction.WasPressedThisFrame() && !attackLocked)
        {
            // Decide attack type by vertical input at the moment of pressing attack.
            // Threshold prevents accidental up/down from small sticks.
            const float verticalThreshold = 0.5f;
            if (moveVec.y > verticalThreshold)
                PerformAttackUp();
            else if (moveVec.y < -verticalThreshold)
                PerformAttackDown();
            else
                PerformAttackNormal();
        }

        // Heavy attack input
        if (hAttackAction != null && hAttackAction.triggered && isGrounded && !attackLocked)
        {
            PerformHeavyAttack();
        }

        // Heal and skill input
        if (healAction != null && healAction.WasPressedThisFrame() && isGrounded && !skillLocked)
        {
            skillManager.UseSkillById(0);
        }

        if (skillAction != null && skillAction.WasPressedThisFrame() && !skillLocked)
        {
            skillManager.UseActiveSkill();
        }

        bool canFirstJump = (isGrounded || (Time.time - lastGroundedTime <= coyoteTime)) && !isDashing;
        bool canExtraJump = (!canFirstJump && extraJumpCount < maxExtraJumpCount) && isDashing;

        // Start jump
        if (jumpAction.WasPressedThisFrame() && canFirstJump)
        {
            Jump();
            isJumping = true;
            jumpHoldTimer = 0f;
        }
        else if (jumpAction.WasPressedThisFrame() && canExtraJump)
        {
            Jump();
            extraJumpCount++;
            isJumping = true;
            jumpHoldTimer = 0f;
        }

        // Handle jump hold for variable height
        if (isJumping && !movementLocked)
        {
            if (jumpAction.IsPressed() && jumpHoldTimer < jumpHoldTimeMax && rb.linearVelocityY > 0)
            {
                rb.linearVelocityY += jumpHoldForce * Time.deltaTime;
                jumpHoldTimer += Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (movementLocked) return;

        // If dashing, apply dash movement and manage dash timer; otherwise normal running logic
        if (isDashing)
        {
            dashTimeLeft -= Time.fixedDeltaTime;
            float dir = isFacingRight ? 1f : -1f;
            // lock horizontal dash velocity and optionally zero vertical component
            rb.linearVelocityX = dir * dashSpeed;
            rb.linearVelocityY = 0f;

            if (dashTimeLeft <= 0f)
                EndDash();
        }
        else
        {
            Running();

            // Double gravity after peak (when falling)
            if (rb.linearVelocityY < 0)
            {
                rb.gravityScale = fallGravityMultiplier;
            }
            else
            {
                rb.gravityScale = originalGravityMultiplier;
            }

            // Cap falling speed
            if (rb.linearVelocityY < maxFallSpeed)
            {
                rb.linearVelocityY = maxFallSpeed;
            }
        }
    }

    private void Running()
    {
        const float deadZone = 0.001f;
        if (Mathf.Abs(moveAmt) > deadZone)
        {
            rb.linearVelocityX = moveAmt * moveSpeed;
        }
    }

    private void Jump()
    {
        rb.linearVelocityY = jumpForce;
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimeLeft = dashDuration;
        lastDashTime = Time.time;
        rb.gravityScale = 0f;
        isJumping = false;
        rb.linearVelocityY = 0f;
    }

    private void EndDash()
    {
        isDashing = false;
        // restore gravity scale
        rb.gravityScale = originalGravityMultiplier;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = true;
            extraJumpCount = 0;
            lastGroundedTime = Time.time;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = false;
            lastGroundedTime = Time.time;
        }
    }

    // Attack callbacks
    private void PerformAttackNormal()
    {
        Debug.Log("Attack: Normal");
        // Ensure animator has a trigger named "AttackNormal"
        //animator?.SetTrigger("AttackNormal");
        // TODO: spawn hitbox or call attack detection here
    }

    private void PerformAttackUp()
    {
        Debug.Log("Attack: Up");
        // Ensure animator has a trigger named "AttackUp"
        //animator?.SetTrigger("AttackUp");
        // TODO: spawn upward hitbox or call attack detection here
    }

    private void PerformAttackDown()
    {
        Debug.Log("Attack: Down");
        // Ensure animator has a trigger named "AttackDown"
        //animator?.SetTrigger("AttackDown");
        // TODO: spawn downward hitbox or call attack detection here
    }

    private void PerformHeavyAttack()
    {
        Debug.Log("Heavy Attack");
        // Ensure animator has a trigger named "HeavyAttack"
        //animator?.SetTrigger("HeavyAttack");
    }

    // Flip the character to face the direction of movement
    private void FlipDirection()
    {
        if (moveAmt > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (moveAmt < 0 && isFacingRight)
        {
            Flip();
        }
    }
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
