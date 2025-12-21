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
    bool isAwaken;

    public Fly0SleepState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        if (!isAwaken)
        {
            if (IsTargetValid())
            {
                isAwaken = true;
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
    private Health targetHealth;
    private Vector2 chaseVel;

    public Fly0ChaseState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        animStateMachine.ChangeState(fly0.animFlyingState);

        if(IsTargetValid())
            targetHealth = targetHandler.CurrentTarget.GetComponent<Health>();
    }

    public override void Update()
    {
        base.Update();

        if (targetHealth != null && targetHealth.IsDead()) fly0.StopVelocity();

        if (targetHandler.GetDistanceToTarget() > fly0.stopChaseDistance)
            chaseVel = targetHandler.GetDirectionToTarget() * fly0.moveSpeed;
        else chaseVel = Vector2.zero;

        if (IsCurrentAttackReady())
        {
            logicStateMachine.ChangeState(fly0.repositionToAttackState);
        }
    }

    public override void FixedUpdate()
    {
        fly0.SetVelocity(chaseVel);
    }
}

public class Fly0RepositionToAttackState : Fly0State
{
    private Vector2 nextPos;
    private float repositionRange = 7f;
    private float restTime = 0.2f;

    public Fly0RepositionToAttackState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        AttackOrReposition();
    }

    public override void Update()
    {
        base.Update();

        if (Vector2.Distance(fly0.transform.position, nextPos) < 0.1f)
        {
            fly0.StopVelocity();

            AttackOrReposition();
        }
        else
        {
            Vector2 dir = (nextPos - (Vector2)fly0.transform.position).normalized;
            fly0.SetVelocity(dir * fly0.moveSpeed);
        }
    }

    private void AttackOrReposition()
    {
        if (IsCurrentTargetInAttackArea())
            logicStateMachine.ChangeState(fly0.attackState);
        else
            CaculateNextPos();
    }

    private void CaculateNextPos()
    {
        if (!IsTargetValid()) return;

        nextPos = targetHandler.GetTargetPosition();

        Vector2 randomPosWithinTarget = Random.insideUnitCircle * repositionRange;
        randomPosWithinTarget.y = Mathf.Abs(randomPosWithinTarget.y);

        nextPos += randomPosWithinTarget;
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
        fly0.StopVelocity();
        FlipToTarget();
        animStateMachine.ChangeState(fly0.animAttackState);

        if (IsTargetValid())
        {
            attackSet.CurrentAttack.AttackPointLookAt(targetHandler.CurrentTarget);
        }
    }

    public override void Update()
    {
        base.Update();

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

        if (stateTimer < 0)
        {
            logicStateMachine.ChangeState(fly0.chaseState);
        }
    }
}