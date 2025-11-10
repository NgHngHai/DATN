using UnityEngine;

public class SnatcherState : EnemyOffensiveState
{
    protected EnemySnatcher snatcher;

    public SnatcherState(Enemy enemy) : base(enemy)
    {
        snatcher = enemy as EnemySnatcher;
    }
}

public class SnatcherOnWallState : SnatcherState
{
    public SnatcherOnWallState(Enemy enemy) : base(enemy)
    {
    }

    public override void Update()
    {
        base.Update();
        if (IsTargetValid())
        {
            logicStateMachine.ChangeState(snatcher.jumpTargetState);
        }
    }
}

public class SnatcherJumpOnTargetState : SnatcherState
{
    private Rigidbody2D rb;
    private Collider2D col;

    private bool hasJumped;
    private bool hasLanded;

    public SnatcherJumpOnTargetState(Enemy enemy) : base(enemy)
    {
        rb = snatcher.GetComponent<Rigidbody2D>();
        col = snatcher.GetComponent<Collider2D>();
    }

    public override void Enter()
    {
        base.Enter();

        snatcher.OnTriggerEntered += HandleTriggerEnter;
        snatcher.OnTriggerExited += HandleTriggerExit;

        JumpAtPlayer();
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0 && hasLanded)
        {
            logicStateMachine.ChangeState(snatcher.chaseState);
        }

    }

    public override void Exit()
    {
        base.Exit();

        snatcher.OnTriggerEntered -= HandleTriggerEnter;
        snatcher.OnTriggerExited -= HandleTriggerExit;
    }

    private void HandleTriggerEnter(Collider2D collision)
    {
        bool hitTarget = PhysicsUtils.IsGameObjectInLayer(collision.gameObject, snatcher.targetMask);

        bool hitLand = PhysicsUtils.IsGameObjectInLayer(collision.gameObject, snatcher.groundMask);

        if (hitTarget || hitLand)
        {
            CrossJumpWhenLanding();

            if (!hitTarget) return;

            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null)
                damageable.TakeDamage(snatcher.jumpDamage);
        }
    }

    private void HandleTriggerExit(Collider2D collision)
    {
        if (hasJumped) return;

        if (PhysicsUtils.IsGameObjectInLayer(collision.gameObject, snatcher.groundMask))
        {
            hasJumped = true;
        }
    }

    private void CrossJumpWhenLanding()
    {
        snatcher.StopVelocity();

        float angle = snatcher.landJumpAngle;
        Vector2 baseDir = Quaternion.Euler(0, 0, angle) * Vector2.right;

        if (Random.Range(0, 2) == 0)
            baseDir.x = -baseDir.x;

        rb.AddForce(baseDir.normalized * snatcher.landJumpForce, ForceMode2D.Impulse);

        stateTimer = 1f;
        hasLanded = true;
        col.isTrigger = false;
    }

    private void JumpAtPlayer()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;

        Vector2 startPos = snatcher.transform.position;
        Vector2 targetPos = targetHandler.CurrentTarget.position;

        float g = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);
        float t = snatcher.jumpTime;

        Vector2 displacement = targetPos - startPos;

        float vx = displacement.x / t;
        float vy = displacement.y / t + 0.5f * g * t;

        Vector2 velocity = new Vector2(vx, vy);
        rb.AddForce(velocity * rb.mass, ForceMode2D.Impulse);

        snatcher.FlipOnVelocityX();
    }
}

public class SnatcherPatrolState : SnatcherState
{
    EnemyPatrolBehavior patrolBehavior;
    public SnatcherPatrolState(Enemy enemy) : base(enemy)
    {
        patrolBehavior = snatcher.GetComponent<EnemyPatrolBehavior>();
    }

    public override void Enter()
    {
        base.Enter();
        patrolBehavior.StartPatrolProcess();
    }

    public override void Update()
    {
        base.Update();
        if (IsTargetValid())
        {
            logicStateMachine.ChangeState(snatcher.chaseState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        patrolBehavior.StopPatrolProcess();
    }
}

public abstract class SnatcherCombatState : SnatcherState
{
    public SnatcherCombatState(Enemy enemy) : base(enemy)
    {
    }

    public override void Update()
    {
        base.Update();
        if (IsTargetValid())
        {
            UpdateIfTargetNotNull();
        }
        else
        {
            logicStateMachine.ChangeState(snatcher.patrolState);
        }
    }

    protected abstract void UpdateIfTargetNotNull();
}

public class SnatcherChaseState : SnatcherCombatState
{
    Vector2 chaseVel;
    public SnatcherChaseState(Enemy enemy) : base(enemy)
    {
    }

    protected override void UpdateIfTargetNotNull()
    {
        int targetDirX = targetHandler.GetHorizontalDirectionToTarget();
        chaseVel.x = snatcher.moveSpeed * targetDirX;

        snatcher.SetVelocity(chaseVel);

        if (IsTargetInCurrentAttackArea(targetHandler.CurrentTarget))
        {
            snatcher.StopVelocity();

            if (stateTimer > 0) return;

            stateTimer = snatcher.attackCooldown;
            logicStateMachine.ChangeState(snatcher.attackState);
        }
    }
}

public class SnatcherAttackState : SnatcherCombatState
{
    public SnatcherAttackState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        TryCurrentAttack();
        logicStateMachine.ChangeState(snatcher.chaseState);
    }

    protected override void UpdateIfTargetNotNull()
    {
    }
}
