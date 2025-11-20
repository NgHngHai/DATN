using UnityEngine;

public class FalS0State : EnemyOffensiveState
{
    protected EnemyFalS0 fals0;
    protected int meleeActtackIndex = 0;
    protected int counterActtackIndex = 1;

    public FalS0State(Enemy enemy) : base(enemy)
    {
        fals0 = enemy as EnemyFalS0;
    }
}

public class FalS0ChaseState : FalS0State
{
    Vector2 chaseVel;

    private float decideTime = 0.5f;

    public FalS0ChaseState(Enemy enemy) : base(enemy)
    {
    }

    public override void Update()
    {
        base.Update();

        if (!IsTargetValid())
        {
            fals0.StopVelocity();
            return;
        }

        ChaseTarget();
        AttackOrBlock();
    }

    private void ChaseTarget()
    {
        int targetDirX = targetHandler.GetHorizontalDirectionToTarget();
        chaseVel.x = fals0.moveSpeed * targetDirX;

        fals0.SetVelocity(chaseVel);
    }

    private void AttackOrBlock()
    {
        if (IsTargetInCurrentAttackArea(targetHandler.CurrentTarget))
        {
            fals0.StopVelocity();

            if (IsCurrentAttackReady())
                logicStateMachine.ChangeState(fals0.meleeAttackState);
        }
        else if(stateTimer < 0)
        {
            stateTimer = decideTime;
            bool willBlock = Random.Range(0, 2) == 1 ? true : false;
            if (willBlock)
                logicStateMachine.ChangeState(fals0.blockState);
        }
    }
}

public class FalS0MeleeAttackState : FalS0State
{
    public FalS0MeleeAttackState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        attackSet.ChangeAttackType(0);
        TryCurrentAttack();
    }

    public override void Update()
    {
        base.Update();
        // TO DO: Change state when trigger is called (if possible ?)
    }
}

public class FalS0BlockState : FalS0State
{
    private Vector2 blockDurationLimit = new Vector2(1, 3);
    public FalS0BlockState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        fals0.StopVelocity();
        stateTimer = Random.Range(blockDurationLimit.x, blockDurationLimit.y);
    }

    public override void Update()
    {
        base.Update();

        FlipToTarget();

        if (stateTimer < 0)
            logicStateMachine.ChangeState(fals0.chaseState);
    }
}

public class FalS0CounterAttackState : FalS0State
{
    public FalS0CounterAttackState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        attackSet.ChangeAttackType(1);
        TryCurrentAttack();
    }

    public override void Update()
    {
        base.Update();
        // TO DO: Change state when trigger is called (if possible ?)
    }
}