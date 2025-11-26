//using System;
//using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Entity
{
    public static PlayerController Instance { get; private set; }

    // Component references
    public PlayerSkillManager skillManager;
    public Health playerHealth;

    [Header("Spawnpoint")]
    // Room spawnpoint ID
    public string currentRoomID = "Room0";

    // Movement variables
    [Header("Movement Variables")]
    [Tooltip("When true, external systems own velocity. Player input should not write velocity.")]
    public float moveSpeed = 7f;
    public float externalVelocityX = 0f;
    private float moveAmt;

    // Jumping
    [Header("Jump Variables")]
    public float jumpForce = 5f;
    public int maxExtraJumpCount = 1;
    public bool isGrounded = false;
    private int extraJumpCount = 0;
    // Coyote time
    public float coyoteTime = 0.1f;
    private float lastGroundedTime = -1f;
    // Advanced jumping
    public float jumpHoldTimeMax = 0.2f;     // Maximum time jump can be held
    public float jumpHoldForce = 50f;        // Extra force applied while holding jump
    public float originalGravityMultiplier = 5f; // Original gravity multiplier
    public float fallGravityMultiplier = 8f; // Gravity multiplier after peak
    public float maxFallSpeed = -50f;        // Maximum downward velocity
    private float jumpHoldTimer = 0f;
    private bool isJumping = false;

    // Ground/Wall detection
    [Header("Ground Check")]
    [Tooltip("Minimum upward normal.y to count as ground. Raise if vertical walls sometimes register as ground.")]
    [SerializeField] private float wallNormalMinX = 0.6f;
    [Tooltip("Minimum absolute normal.x to count as a wall when not ground.")]
    [SerializeField] private float groundNormalMinY = 0.7f;
    [SerializeField] private Vector2 groundCheckOffset = new Vector2(0f, -0.9f);
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.8f, 0.15f);
    [SerializeField] private LayerMask groundLayers = ~0;
    private bool isTouchingWall;
    private bool wallOnLeft;
    private bool wallOnRight;

    [Header("Air Friction Control")]
    [Tooltip("Frictionless (or low friction) material applied only while airborne after a jump.")]
    public PhysicsMaterial2D frictionlessMaterial;

    // Attacking & skills
    [Tooltip("When true, cannot input skills.")]
    public bool skillLocked = false;
    private bool isAttacking;
    [SerializeField] private bool isCountering = false;
    [Tooltip("When true, can perform counter attack.")]

    // Dashing
    [Header("Dash Variables")]
    public float dashSpeed = 25f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.5f;
    public bool isDashing = false;
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
    private InputAction parryAction;
    private InputAction counterAction;

    private InputAction healAction;
    private InputAction skillAction;

    // Animation states
    [Header("Animation States")]
    public AnimationState idleState;
    public AnimationState runState;
    public AnimationState jumpState;
    public AnimationState doubleJumpState;
    public AnimationState dashState;
    public AnimationState fallState;
    public AnimationState ascendState;

    public AnimationState normalAttackState;
    public AnimationState heavyAttackState;
    public AnimationState parryState;
    public AnimationState counterState;

    public AnimationState hurtState;
    public AnimationState deathState;

    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
    }

    protected override void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        base.Awake();
        animator = GetComponent<Animator>();            // Lấy component Animator
        playerHealth = GetComponent<Health>();          // Lấy component Health

        // Gán từng state, tên animBoolName phải trùng với parameter trong Animator
        idleState = new AnimationState(this, "idle", true);
        runState = new AnimationState(this, "run", true);
        jumpState = new AnimationState(this, "jump", true);
        doubleJumpState = new AnimationState(this, "doubleJump", true);
        dashState = new AnimationState(this, "dash", true);
        fallState = new AnimationState(this, "fall", true);
        ascendState = new AnimationState(this, "ascend", true);

        normalAttackState = new AnimationState(this, "nAttack", false);
        heavyAttackState = new AnimationState(this, "hAttack", false);
        parryState = new AnimationState(this, "parry", true);
        counterState = new AnimationState(this, "counter", false);

        hurtState = new AnimationState(this, "hurt", true);
        deathState = new AnimationState(this, "dead", true);

        animStateMachine.Initialize(idleState);         // Bắt đầu ở trạng thái Idle

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
                parryAction = map.FindAction("Parry");

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
        parryAction = InputSystem.actions.FindAction("Parry");

        healAction = InputSystem.actions.FindAction("Heal");
        skillAction = InputSystem.actions.FindAction("Skill");
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (animStateMachine.currentState == deathState)
            return; // Dead - no input

        // Read movement input
        Vector2 moveVec = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;

        PerformGroundProbe();

        if (!isDashing && !movementLocked)
        {
            // Get horizontal movement amount
            moveAmt = moveVec.x;
            FlipOnVelocityX();
        }
        else
        {
            // Prevent stale input from writing velocities when locked
            moveAmt = 0f;
        }

        // dash input (uses a simple cooldown)
        bool canDash = !isDashing && !movementLocked && (Time.time - lastDashTime >= dashCooldown);
        if (dashAction != null && dashAction.WasPressedThisFrame() && canDash)
            StartDash();
        if (isDashing)
        {
            animStateMachine.ChangeState(dashState);
            return;
        }

        if (parryAction != null && parryAction.WasPressedThisFrame())
        {
            animStateMachine.ChangeState(parryState);
        }

        // Attack input - can be used while running (does not cancel horizontal control)
        if (attackAction != null && attackAction.WasPressedThisFrame() && !attackLocked)
        {
            // Decide attack type by vertical input at the moment of pressing attack.
            // Threshold prevents accidental up/down from small sticks.
            const float verticalThreshold = 0.5f;
            if (moveVec.y > verticalThreshold)
            {
                isAttacking = true;
                PerformAttackUp();
                return;
            }
            else if (moveVec.y < -verticalThreshold)
            {
                isAttacking = true;
                PerformAttackDown();
                return;
            }
            else
            {
                if (isGrounded)
                {
                    isAttacking = true;
                    PerformAttackNormal();
                    return;
                }
                else
                {
                    // PerformAirAttack();
                    // return;
                }
            }
        }

        // Heavy attack input
        if (hAttackAction != null && hAttackAction.triggered && isGrounded && !attackLocked)
        {
            PerformAttackHeavy();
            return;
        }

        // Heal and skill input
        if (healAction != null && healAction.WasPressedThisFrame() && isGrounded && !skillLocked && !isAttacking)
        {
            skillManager.UseSkillById(0);
        }
         
        if (skillAction != null && skillAction.WasPressedThisFrame() && !skillLocked && !isAttacking)
        {
            skillManager.UseActiveSkill();
        }

        bool canFirstJump = (isGrounded || (Time.time - lastGroundedTime <= coyoteTime)) && !isDashing;
        bool canExtraJump = (!canFirstJump && extraJumpCount < maxExtraJumpCount) && !isDashing;

        // Start jump
        if (jumpAction.WasPressedThisFrame() && canFirstJump && !movementLocked)
        {
            Jump();
            isJumping = true;
            jumpHoldTimer = 0f;
            animStateMachine.ChangeState(jumpState);
            return;
        }
        else if (jumpAction.WasPressedThisFrame() && canExtraJump && !movementLocked)
        {
            animStateMachine.ChangeState(doubleJumpState);
            Jump();
            extraJumpCount++;
            isJumping = true;
            jumpHoldTimer = 0f;
            return;
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

        // OTHER ANIMATION STATE UPDATES
        // Fall state
        if (rb.linearVelocityY < 0 && !isGrounded)
        {
            animStateMachine.ChangeState(fallState);
            return;
        } else if (rb.linearVelocityY > 0 && !isGrounded)
        {
            animStateMachine.ChangeState(ascendState);
            return;
        }

        // Running/Idle state
        if (moveAction.inProgress && moveAmt != 0)
        {
            animStateMachine.ChangeState(runState);
        }
        else
        {
            animStateMachine.ChangeState(idleState);
        }
    }

    private void FixedUpdate()
    {
        // Reduce external velocity over time
        externalVelocityX = Mathf.MoveTowards(externalVelocityX, 0f, 30 * Time.fixedDeltaTime);

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
        float internalVelocityX = moveAmt * moveSpeed;

        if (!isGrounded && isTouchingWall)
            if ((wallOnLeft && internalVelocityX < 0f) || (wallOnRight && internalVelocityX > 0f))
                internalVelocityX = 0f;

        rb.linearVelocityX = internalVelocityX + externalVelocityX;
    }

    private void Jump()
    {
        rb.linearVelocityY = jumpForce;
        ApplyAirPhysicsMaterial();
    }

    private void StartDash()
    {
        isDashing = true;
        isJumping = false;

        dashTimeLeft = dashDuration;
        lastDashTime = Time.time;

        rb.gravityScale = 0f;
        rb.linearVelocityY = 0f;

        playerHealth.isInvincible = true;

        if (!isGrounded) ApplyAirPhysicsMaterial();
    }

    private void EndDash()
    {
        isDashing = false;
        // restore gravity scale
        rb.gravityScale = originalGravityMultiplier;

        playerHealth.isInvincible = false;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if ((groundLayers.value & (1 << collision.gameObject.layer)) == 0)
            return;

        ApplyAirPhysicsMaterial();

        wallOnLeft = false;
        wallOnRight = false;
        isTouchingWall = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if ((groundLayers.value & (1 << collision.gameObject.layer)) == 0)
            return;

        bool foundWallLeft = false;
        bool foundWallRight = false;

        for (int i = 0; i < collision.contactCount; i++)
        {
            var contact = collision.GetContact(i);
            Vector2 n = contact.normal;

            // Treat near-vertical normals as walls
            if (Mathf.Abs(n.x) >= wallNormalMinX && n.y < groundNormalMinY)
            {
                if (n.x > 0f) foundWallLeft = true;
                if (n.x < 0f) foundWallRight = true;
            }
        }

        wallOnLeft = foundWallLeft;
        wallOnRight = foundWallRight;
        isTouchingWall = wallOnLeft || wallOnRight;

        if (!isGrounded)
        {
            ApplyAirPhysicsMaterial();
        }
        else {
            ClearPhysicsMaterial();
        }
    }

    private void PerformGroundProbe()
    {
        bool wasGrounded = isGrounded;
        Collider2D hit = Physics2D.OverlapBox((Vector2)transform.position + groundCheckOffset, groundCheckSize, 0f, groundLayers);
        isGrounded = hit != null;

        if (isGrounded && !wasGrounded)
        {
            extraJumpCount = 0;
            lastGroundedTime = Time.time; // landed
        }
        else if (!isGrounded && wasGrounded)
        {
            lastGroundedTime = Time.time; // start coyote
        }
    }

    private void ApplyAirPhysicsMaterial()
    {
        if (frictionlessMaterial == null) return;
        rb.sharedMaterial = frictionlessMaterial;
    }

    private void ClearPhysicsMaterial()
    {
        rb.sharedMaterial = null;
    }

    // Attack callbacks
    private void PerformAttackNormal()
    {
        movementLocked = true;
        animStateMachine.ChangeState(normalAttackState);
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

    private void PerformAttackHeavy()
    {
        Debug.Log("Heavy Attack");
        // Ensure animator has a trigger named "HeavyAttack"
        //animator?.SetTrigger("HeavyAttack");
    }

    private void PerformAirAttack()
    {
        Debug.Log("Air Attack");
        // Ensure animator has a trigger named "AirAttack"
        //animator?.SetTrigger("AirAttack");
    }

    public void UnlockPostAttack()
    {
        movementLocked = false;
        isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((Vector2)transform.position + groundCheckOffset, groundCheckSize);
    }
}