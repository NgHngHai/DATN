using UnityEngine;

public class BoomState : EnemyState
{
    protected EnemyBoom boom;
    public BoomState(Enemy enemy) : base(enemy)
    {
        boom = enemy as EnemyBoom;
    }
}

public class BoomPatrolState : BoomState
{
    protected EnemyPatrolBehavior patrolBehavior;
    public BoomPatrolState(Enemy enemy) : base(enemy)
    {
        patrolBehavior = boom.GetComponent<EnemyPatrolBehavior>();
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
            logicStateMachine.ChangeState(boom.targetDetectedState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        patrolBehavior.StopPatrolProcess();
    }
}

public class BoomTargetDetectedState : BoomState
{
    private float jumpForce = 2f;
    private float waitTime = 1f;

    public BoomTargetDetectedState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        FlipToTarget();
        boom.SetVelocity(new Vector2(0, jumpForce));
        stateTimer = waitTime;
    }

    public override void Update()
    {
        base.Update();

        if (!IsTargetValid())
        {
            logicStateMachine.ChangeState(boom.patrolState);
        }
        else if(stateTimer < 0)
        {
            logicStateMachine.ChangeState(boom.dashState);
        }
    }
}

public class BoomDashState : BoomState
{
    private Vector2 dashVel;

    public BoomDashState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = boom.dashDuration;
    }

    public override void Update()
    {
        base.Update();

        DashInFaceDirection();
        ChangeStateOnTargetStatus();

        if (targetHandler.GetDistanceToTarget() < boom.explodeDistance)
        {
            boom.Explode();
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        DashInFaceDirection(); 
    }

    private void DashInFaceDirection()
    {
        float t = boom.dashCurve.Evaluate(1 - (stateTimer / boom.dashDuration));
        float dashSpeed = Mathf.Lerp(0, boom.maxDashSpeed, t);

        dashVel.x = dashSpeed * boom.FacingDir;

        boom.SetVelocity(dashVel);
    }

    private void ChangeStateOnTargetStatus()
    {
        if (stateTimer > 0) return;

        if (targetHandler.CurrentTarget == null)
        {
            logicStateMachine.ChangeState(boom.targetDetectedState);
        }
        else
        {
            logicStateMachine.ChangeState(boom.patrolState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        boom.StopVelocity();
    }
}

