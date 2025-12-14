using UnityEngine;

public class BoomState : EnemyState
{
    protected EnemyBoom boom;
    private ParticleSystem.EmissionModule runningDustEmission;

    public BoomState(Enemy enemy) : base(enemy)
    {
        boom = enemy as EnemyBoom;
        runningDustEmission = boom.runningDustPS.emission;
    }

    protected void SetRunningDustRateOverTime(float rate)
    {
        runningDustEmission.rateOverTime = rate;
    }

    protected void SetAnimatorSpeed(float speed)
    {
        boom.animator.speed = speed;
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
        SetRunningDustRateOverTime(4f);
        SetAnimatorSpeed(1f);
    }

    public override void Update()
    {
        base.Update();
        if (IsTargetValid())
        {
            logicStateMachine.ChangeState(boom.chaseState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        patrolBehavior.StopPatrolProcess();
    }
}

public class BoomChaseState : BoomState
{
    public BoomChaseState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        SetRunningDustRateOverTime(20f);
        SetAnimatorSpeed(2f);
    }

    public override void Update()
    {
        base.Update();

        if (!IsTargetValid())
        {
            logicStateMachine.ChangeState(boom.patrolState);
            return;
        }

        FlipToTarget();

        boom.SetVelocityX(boom.FacingDir * boom.chaseSpeed);

        if(targetHandler.GetDistanceToTarget() < boom.explodeDistance)
        {
            logicStateMachine.ChangeState(boom.explodeState);
        }
    }
}
public class BoomExplodeState : BoomState
{
    public BoomExplodeState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        boom.StopVelocity();
        SetRunningDustRateOverTime(0f);
        SetAnimatorSpeed(1f);
        animStateMachine.ChangeState(boom.animExplodeState);
    }

    public override void Update()
    {
        base.Update();

        if (boom.IsCurrentAnimStateTriggerCalled())
        {
            boom.DestroyItself();
        }
    }
}