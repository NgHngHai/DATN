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
    public AnimationState attackUpState;
    public AnimationState attackDownState;
    public AnimationState heavyAttackState;
    public AnimationState parryState;
    public AnimationState counterState;

    public AnimationState hurtState;
    public AnimationState deathState;

    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();

        // Subscribe to health events to react to hits/death
        if (playerHealth == null) playerHealth = GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.OnDamagedWithReaction.AddListener(HandleDamagedWithReaction);
            playerHealth.OnDeath.AddListener(HandleDeath);
        }
    }

    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();

        // Unsubscribe to avoid duplicate handlers on scene reloads
        if (playerHealth != null)
        {
            playerHealth.OnDamagedWithReaction.RemoveListener(HandleDamagedWithReaction);
            playerHealth.OnDeath.RemoveListener(HandleDeath);
        }
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
        attackUpState = new AnimationState(this, "attackUp", false);
        attackDownState = new AnimationState(this, "attackDown", false);
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

        if (animStateMachine.currentState == hurtState && movementLocked)
        {
            return;
        }

        // Read movement input
        Vector2 moveVec = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;

        PerformGroundProbe();

        if (!isDashing && !movementLocked)
        {
            // Get horizontal movement amount
            moveAmt = moveVec.x;
            FlipOnMoveInput();
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
            isAttacking = true;
            // Decide attack type by vertical input at the moment of pressing attack.
            // Threshold prevents accidental up/down from small sticks.
            const float verticalThreshold = 0.5f;
            if (moveVec.y > verticalThreshold)
            {
                PerformAttackUp();
                return;
            }
            else if (moveVec.y < -verticalThreshold)
            {
                PerformAttackDown();
                return;
            }
            else
            {
                if (isGrounded)
                {
                    PerformAttackNormal();
                    return;
                }
                else
                {
                     PerformAirAttack();
                    return;
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

    private void FlipOnMoveInput()
    {
        if (moveAmt < 0 && isFacingRight)
            Flip();
        else if (moveAmt > 0 && !isFacingRight)
            Flip();
    }

    private void Running()
    {
        float internalVelocityX = moveAmt * moveSpeed;
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
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if ((groundLayers.value & (1 << collision.gameObject.layer)) == 0)
            return;

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
        movementLocked = true;
        animStateMachine.ChangeState(attackUpState);
    }

    private void PerformAttackDown()
    {
        movementLocked = true;
        animStateMachine.ChangeState(attackDownState);
    }

    private void PerformAttackHeavy()
    {
        movementLocked = true;
        animStateMachine.ChangeState(heavyAttackState);
    }

    private void PerformAirAttack()
    {
        Debug.Log("Air Attack");
        // Ensure animator has a trigger named "AirAttack"
        //animator?.SetTrigger("AirAttack");
    }

    // Health event handlers
    private void HandleDamagedWithReaction(int appliedAmount, bool shouldTriggerHitReaction)
    {
        // Only react when damage actually applied and reaction requested
        if (appliedAmount <= 0 || !shouldTriggerHitReaction)
            return;

        if (animStateMachine.currentState == deathState)
            return;

        // Switch to hurt state (knockback is already applied by HurtBox -> Entity.ApplyKnockback)
        animStateMachine.ChangeState(hurtState);
    }

    private void HandleDeath()
    {
        animStateMachine.ChangeState(deathState);
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