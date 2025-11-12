using UnityEngine;

public class RapicitatorState : EnemyOffensiveState
{
    protected EnemyRapicitator rapicitator;

    public RapicitatorState(Enemy enemy) : base(enemy)
    {
        rapicitator = enemy as EnemyRapicitator;
    }
}

public class RapicitatorPatrolState : RapicitatorState
{
    EnemyPatrolBehavior patrolBehavior;

    public RapicitatorPatrolState(Enemy enemy) : base(enemy)
    {
        patrolBehavior = rapicitator.GetComponent<EnemyPatrolBehavior>();
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
            logicStateMachine.ChangeState(rapicitator.chaseState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        patrolBehavior.StopPatrolProcess();
    }
}

public abstract class RapicitatorTargetState : RapicitatorState
{
    public RapicitatorTargetState(Enemy enemy) : base(enemy)
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
            logicStateMachine.ChangeState(rapicitator.patrolState);
        }
    }

    protected abstract void UpdateIfTargetNotNull();
}

public class RapicitatorChaseState : RapicitatorTargetState
{
    Vector2 chaseVel;
    public RapicitatorChaseState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        attackSet.ChangeAttackType<EnemyMeleeAttack>();
    }

    protected override void UpdateIfTargetNotNull()
    {
        int targetDirX = targetHandler.GetHorizontalDirectionToTarget();
        chaseVel.x = rapicitator.chaseSpeed * targetDirX;
        rapicitator.SetVelocity(chaseVel);

        if (IsTargetInCurrentAttackArea(targetHandler.CurrentTarget))
        {
            logicStateMachine.ChangeState(rapicitator.legAttackState);
        }
    }
}

public class RapicitatorFleeState : RapicitatorTargetState
{
    private Vector2 fleeVel;
    private Vector2 startPos;

    public RapicitatorFleeState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        FlipToTarget(true);
        startPos = rapicitator.transform.position;
        fleeVel.x = rapicitator.fleeSpeed * rapicitator.FacingDir;
    }

    protected override void UpdateIfTargetNotNull()
    {
        rapicitator.SetVelocity(fleeVel);
        float distanceTraveled = Vector2.Distance(startPos, rapicitator.transform.position);

        if (rapicitator.IsGroundEdgeOrWallDetected() || distanceTraveled >= rapicitator.minFleeShootLaserDistance)
        {
            logicStateMachine.ChangeState(rapicitator.laserAttackState);
        }
    }
}

public class RapicitatorLegAttackState : RapicitatorState
{
    public RapicitatorLegAttackState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        rapicitator.StopVelocity();

        if (attackSet.ChangeAttackType<EnemyMeleeAttack>())
        {
            TryCurrentAttack();
        }

        logicStateMachine.ChangeState(rapicitator.fleeState);
    }
}

public class RapicitatorLaserAttackState : RapicitatorState
{
    public RapicitatorLaserAttackState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        rapicitator.StopVelocity();
        FlipToTarget();

        if (attackSet.ChangeAttackType<EnemyLaserAttack>())
        {
            Vector2 lookDir = enemy.FacingDir == 1 ? Vector2.right : Vector2.left;
            attackSet.CurrentAttack.AttackPointLookAtDirection(lookDir);

            TryCurrentAttack();
        }

        stateTimer = rapicitator.shootLaserRestTime;
    }

    public override void Update()
    {
        base.Update();

        if(stateTimer < 0)
        {
            logicStateMachine.ChangeState(rapicitator.chaseState);
        }
    }
}
