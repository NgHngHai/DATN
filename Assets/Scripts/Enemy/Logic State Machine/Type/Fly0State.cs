using UnityEngine;

public class Fly0State : EnemyOffensiveState
{
    protected EnemyFly0 fly0;

    public Fly0State(Enemy enemy) : base(enemy)
    {
        fly0 = enemy as EnemyFly0;
    }
}

public class Fly0SleepState : Fly0State
{
    bool isSleeping = true;

    public Fly0SleepState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = 1;
    }

    public override void Update()
    {
        if (isSleeping)
        {
            if (IsTargetValid()) isSleeping = false;
            return;
        }

        base.Update();
        if (stateTimer < 0)
        {
            logicStateMachine.ChangeState(fly0.chaseState);
        }
    }
}

public class Fly0HoverAroundState : Fly0State
{
    Vector2 nextHoverPosition;

    public Fly0HoverAroundState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = fly0.hoverRestTime;
        nextHoverPosition = fly0.transform.position;
    }

    public override void Update()
    {
        if (targetHandler.GetDistanceToTarget() > fly0.hoverAroundDistance)
        {
            logicStateMachine.ChangeState(fly0.chaseState);
        }
        else
        {
            HoveringAroundTarget();
        }
    }

    private void HoveringAroundTarget()
    {
        if (Vector2.Distance(fly0.transform.position, nextHoverPosition) < 0.1f)
        {
            base.Update();

            fly0.StopVelocity();

            if (stateTimer < 0)
                HoverOrAttack();
        }
        else
        {
            Vector2 moveDir = (nextHoverPosition - (Vector2)fly0.transform.position).normalized;
            fly0.SetVelocity(moveDir * fly0.hoverSpeed);
        }
    }

    private void HoverOrAttack()
    {
        if (IsCurrentAttackReady() && IsTargetInCurrentAttackArea(targetHandler.CurrentTarget))
        {
            logicStateMachine.ChangeState(fly0.attackState);
        }
        else
        {
            ChooseNextHoverPoint();
        }
    }

    void ChooseNextHoverPoint()
    {
        stateTimer = fly0.hoverRestTime;
        nextHoverPosition = targetHandler.CurrentTarget.position;
        nextHoverPosition += (Random.insideUnitCircle * fly0.hoverAroundDistance);
    }
}

public class Fly0ChaseState : Fly0State
{
    public Fly0ChaseState(Enemy enemy) : base(enemy)
    {
    }

    public override void Update()
    {
        base.Update();

        Vector2 chaseVel = targetHandler.GetDirectionToTarget() * fly0.chaseSpeed;
        fly0.SetVelocity(chaseVel);

        if (targetHandler.GetDistanceToTarget() < fly0.hoverAroundDistance)
        {
            logicStateMachine.ChangeState(fly0.hoverAroundState);
        }
    }
}

public class Fly0AttackState : Fly0State
{
    public Fly0AttackState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        attackSet.CurrentAttack.AttackPointLookAt(targetHandler.CurrentTarget);
        TryCurrentAttack();
        logicStateMachine.ChangeState(fly0.hoverAroundState);
    }
}

