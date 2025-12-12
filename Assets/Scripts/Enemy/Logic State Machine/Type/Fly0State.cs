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
    }

    public override void Update()
    {
        if (isSleeping)
        {
            if (IsTargetValid())
            {
                isSleeping = false;
                animStateMachine.ChangeState(fly0.animAwakeState);
            }
            return;
        }

        base.Update();

        if (fly0.IsCurrentAnimStateTriggerCalled())
        {
            logicStateMachine.ChangeState(fly0.chaseState);
        }
    }
}

public class Fly0ChaseState : Fly0State
{
    Vector2 chaseVel;

    public Fly0ChaseState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        if (targetHandler.GetDistanceToTarget() < fly0.hoverAroundDistance)
        {
            logicStateMachine.ChangeState(fly0.hoverAroundState);
            return;
        }

        chaseVel = targetHandler.GetDirectionToTarget() * fly0.moveSpeed;
    }

    public override void FixedUpdate()
    {
        fly0.SetVelocity(chaseVel);
    }
}

public class Fly0HoverAroundState : Fly0State
{
    Vector2 nextHoverPosition;
    Vector2 moveDir;

    public Fly0HoverAroundState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        base.Enter();
        animStateMachine.ChangeState(fly0.animFlyingState);
        stateTimer = fly0.hoverRestTime;
        nextHoverPosition = fly0.transform.position;
        moveDir = Vector2.zero;
    }

    public override void Update()
    {
        if (targetHandler.GetDistanceToTarget() > fly0.hoverAroundDistance)
        {
            logicStateMachine.ChangeState(fly0.chaseState);
            return;
        }

        if (Vector2.Distance(fly0.transform.position, nextHoverPosition) < 0.1f)
        {
            base.Update();

            fly0.StopVelocity();
            moveDir = Vector2.zero;

            if (stateTimer < 0)
                HoverOrAttack();
        }
        else
        {
            moveDir = (nextHoverPosition - (Vector2)fly0.transform.position).normalized;
        }
    }

    public override void FixedUpdate()
    {
        if (moveDir != Vector2.zero)
            fly0.SetVelocity(moveDir * fly0.moveSpeed);
    }

    private void HoverOrAttack()
    {
        if (IsCurrentAttackReady() && IsTargetInCurrentAttackArea(targetHandler.CurrentTarget))
            logicStateMachine.ChangeState(fly0.attackState);
        else
            ChooseNextHoverPoint();
    }

    void ChooseNextHoverPoint()
    {
        stateTimer = fly0.hoverRestTime;
        Vector2 randomLocalPos = (Random.insideUnitCircle * fly0.hoverAroundDistance);
        nextHoverPosition = targetHandler.GetTargetPosition() + randomLocalPos;
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
        animStateMachine.ChangeState(fly0.animAttackState);
    }

    public override void Update()
    {
        base.Update();

        if (IsTargetValid())
        {
            attackSet.CurrentAttack.AttackPointLookAt(targetHandler.CurrentTarget);
            FlipToTarget();
        }

        if (fly0.IsCurrentAnimStateTriggerCalled())
        {
            logicStateMachine.ChangeState(fly0.restState);
        }
    }

}

public class Fly0RestState : Fly0State
{
    public Fly0RestState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        animStateMachine.ChangeState(fly0.animFlyingState);
        stateTimer = fly0.attackRestTime;
    }

    public override void Update()
    {
        base.Update();
        if(stateTimer < 0)
        {
            logicStateMachine.ChangeState(fly0.chaseState);
        }
    }
}