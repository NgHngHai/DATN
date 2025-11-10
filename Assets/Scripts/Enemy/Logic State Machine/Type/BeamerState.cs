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
    }

    public override void Update()
    {
        base.Update();

        if (!IsTargetValid()) return;

        FlipToTarget();

        if (IsTargetInCurrentAttackArea(targetHandler.CurrentTarget))
        {
            logicStateMachine.ChangeState(beamer.attackState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        health.isInvincible = false;
    }
}

public class BeamerAttackState : BeamerState
{
    private int currentConsecutiveAttack;

    public BeamerAttackState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = beamer.firstConsecutiveAttackDelay;
        currentConsecutiveAttack = 0;
    }

    public override void Update()
    {
        base.Update();

        TrakingTarget();
        AttackConsecutively();
    }

    private void TrakingTarget()
    {
        if (!IsTargetValid()) return;

        FlipToTarget();
        attackSet.CurrentAttack.AttackPointLookAt(targetHandler.CurrentTarget);
    }

    private void AttackConsecutively()
    {
        if (stateTimer > 0) return;

        bool isConsecutiveAttackFinished = currentConsecutiveAttack >= beamer.consecutiveAttackCount;

        if (!isConsecutiveAttackFinished)
        {
            TryCurrentAttack();
            currentConsecutiveAttack++;

            stateTimer = isConsecutiveAttackFinished ? beamer.restTime : beamer.attackInterval;
        }
        else
        {
            logicStateMachine.ChangeState(beamer.observationState);
        }
    }

}