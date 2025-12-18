using UnityEngine;

public class BeamerState : EnemyOffensiveState
{
    protected EnemyBeamer beamer;

    public BeamerState(Enemy enemy) : base(enemy)
    {
        beamer = enemy as EnemyBeamer;
    }
}

public class BeamerObservationState : BeamerState
{
    private Health health;
    public BeamerObservationState(Enemy enemy) : base(enemy)
    {
        health = enemy.GetComponent<Health>();
    }

    public override void Enter()
    {
        base.Enter();
        health.isInvincible = true;
        animStateMachine.ChangeState(beamer.animClosedState);
    }

    public override void Update()
    {
        base.Update();

        if (!IsTargetValid() || !beamer.IsCurrentAnimStateTriggerCalled()) return;

        FlipToTarget();

        if (CanAttackTarget())
        {
            logicStateMachine.ChangeState(beamer.attackState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        health.isInvincible = false;
    }

    private bool CanAttackTarget()
    {
        bool isTargetInAttackDistance = targetHandler.GetDistanceToTarget() <= beamer.startAttackDistance;
        return isTargetInAttackDistance && IsCurrentTargetInAttackArea();
    }
}

public class BeamerAttackState : BeamerState
{
    public BeamerAttackState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        animStateMachine.ChangeState(beamer.animAttackState);
    }

    public override void Update()
    {
        base.Update();

        if (beamer.IsCurrentAnimStateTriggerCalled())
        {
            logicStateMachine.ChangeState(beamer.restState);
        }

        TrakingTarget();
    }

    private void TrakingTarget()
    {
        if (!IsTargetValid()) return;

        FlipToTarget();
        attackSet.CurrentAttack.AttackPointLookAt(targetHandler.CurrentTarget);
    }
}

public class BeamerRestState : BeamerState
{
    public BeamerRestState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = beamer.restTime;
        animStateMachine.ChangeState(beamer.animRestState);
    }

    public override void Update()
    {
        base.Update();
        if (stateTimer < 0)
            logicStateMachine.ChangeState(beamer.observationState);
    }
}
