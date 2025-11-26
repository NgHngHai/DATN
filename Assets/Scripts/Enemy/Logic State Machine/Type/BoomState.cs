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

    public override void Update()
    {
        base.Update();

        if (!IsTargetValid())
        {
            logicStateMachine.ChangeState(boom.patrolState);
            return;
        }

        FlipToTarget();

        Vector2 chaseVel = new Vector2(boom.FacingDir * boom.chaseSpeed, 0);
        boom.SetVelocity(chaseVel);

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