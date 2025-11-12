using UnityEngine;

public class SnatcherState : EnemyOffensiveState
{
    protected EnemySnatcher snatcher;
    protected Rigidbody2D rb;


    public SnatcherState(Enemy enemy) : base(enemy)
    {
        snatcher = enemy as EnemySnatcher;
        rb = snatcher.rb;
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
    SnatcherSecondaryCollider secondaryCollider;

    public SnatcherJumpOnTargetState(Enemy enemy) : base(enemy)
    {
        secondaryCollider = snatcher.secondaryCollider;
    }

    public override void Enter()
    {
        base.Enter();

        secondaryCollider.OnLandingSuccess += OnLandingSuccess;
        JumpAtTarget();
    }

    public override void Exit()
    {
        base.Exit();

        secondaryCollider.OnLandingSuccess -= OnLandingSuccess;
    }

    private void OnLandingSuccess()
    {
        logicStateMachine.ChangeState(snatcher.reboundState);
    }


    private void JumpAtTarget()
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
    }
}

public class SnatcherReboundState : SnatcherState
{
    private Collider2D col;

    public SnatcherReboundState(Enemy enemy) : base(enemy)
    {
        col = snatcher.GetComponent<Collider2D>();
    }

    public override void Enter()
    {
        base.Enter();
        ReboundWhenLanding();

        // Small delay to prevent state change immediately
        stateTimer = 0.2f;
    }

    public override void Update()
    {
        base.Update();

        if(stateTimer < 0 && snatcher.IsGrounded())
        {
            logicStateMachine.ChangeState(snatcher.chaseState);
        }
    }

    private void ReboundWhenLanding()
    {
        snatcher.StopVelocity();

        float angle = snatcher.reboundAngle;
        Vector2 baseDir = Quaternion.Euler(0, 0, angle) * Vector2.right;

        if (Random.Range(0, 2) == 0)
            baseDir.x = -baseDir.x;

        rb.AddForce(baseDir.normalized * snatcher.reboundForce, ForceMode2D.Impulse);

        stateTimer = 1f;
        col.isTrigger = false;
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

public class SnatcherChaseState : SnatcherState
{
    Vector2 chaseVel;
    public SnatcherChaseState(Enemy enemy) : base(enemy)
    {
    }

    public override void Update()
    {
        base.Update();

        if (!IsTargetValid())
        {
            logicStateMachine.ChangeState(snatcher.patrolState);
            return;
        }

        ChaseTarget();
        AttackTargetIfPossible();
    }

    private void ChaseTarget()
    {
        int targetDirX = targetHandler.GetHorizontalDirectionToTarget();
        chaseVel.x = snatcher.moveSpeed * targetDirX;

        snatcher.SetVelocity(chaseVel);
    }

    private void AttackTargetIfPossible()
    {
        if (IsTargetInCurrentAttackArea(targetHandler.CurrentTarget))
        {
            snatcher.StopVelocity();

            if (IsCurrentAttackReady())
                logicStateMachine.ChangeState(snatcher.attackState);
        }
    }
}

public class SnatcherAttackState : SnatcherState
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
}
