using UnityEngine;

public class BowstringState : EnemyOffensiveState
{
    protected BossBowstring bowstring;

    public BowstringState(Enemy enemy) : base(enemy)
    {
        bowstring = enemy as BossBowstring;
    }

    protected Vector2 GetItselfPos() => bowstring.transform.position;
}

public class BowstringAppearState : BowstringState
{
    public BowstringAppearState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        base.Enter();
        animStateMachine.ChangeState(bowstring.animAppearState);
    }

    public override void Update()
    {
        base.Update();

        if (bowstring.IsCurrentAnimStateTriggerCalled())
            logicStateMachine.ChangeState(bowstring.thinkState);
    }
}

public class BowstringThinkState : BowstringState
{
    private float willBurrowAttackDistance = 10;

    public BowstringThinkState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        base.Enter();

        stateTimer = Random.Range(bowstring.thinkTimeRange.x, bowstring.thinkTimeRange.y);
        animStateMachine.ChangeState(bowstring.animIdleState);
        bowstring.StopVelocity();
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
            DecideNextLogicState();
    }

    private void DecideNextLogicState()
    {
        if (!IsTargetValid()) return;

        EnemyState nextState = bowstring.approachState;

        // Only approach
        if (Random.value > bowstring.AggresiveRate)
        {
            logicStateMachine.ChangeState(nextState);
            return;
        }

        if (bowstring.canBurrowAttack)
        {
            float targetDistance = targetHandler.GetDistanceToTarget();

            if (targetDistance > willBurrowAttackDistance) 
                nextState = bowstring.burrowAttackState;
            else
                nextState = Random.value < 0.8f ? bowstring.chaseState : bowstring.burrowAttackState;
        }
        else nextState = bowstring.chaseState;

        if (nextState == bowstring.burrowAttackState)
            bowstring.transform.position = targetHandler.GetTargetPosition();

        logicStateMachine.ChangeState(nextState);
    }
}

public class BowstringApproachState : BowstringState
{
    private Vector2 approachPosition;

    public BowstringApproachState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        base.Enter();
        animStateMachine.ChangeState(bowstring.animMoveState);

        if (IsTargetValid())
        {
            Vector2 targetPos = targetHandler.GetTargetPosition();
            float posX = Mathf.Lerp(GetItselfPos().x, targetPos.x, Random.Range(0.1f, 0.9f));
            approachPosition = new Vector2(posX, targetPos.y + bowstring.approachHeightOffset);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (Vector2.Distance(GetItselfPos(), approachPosition) < 0.1f)
        {
            logicStateMachine.ChangeState(bowstring.thinkState);
            return;
        }

        Vector2 dir = (approachPosition - GetItselfPos()).normalized;
        bowstring.SetVelocity(dir * bowstring.approachSpeed);
    }
}

public class BowstringChaseState : BowstringState
{
    private bool hasDecideNextCloseAttack;
    private bool nextCloseAttackIsPoke;
    private Vector2 attackPosition;

    public BowstringChaseState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        base.Enter();
        animStateMachine.ChangeState(bowstring.animMoveState);
        hasDecideNextCloseAttack = false;
        bowstring.flipOnVelX = true;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!IsTargetValid())
        {
            logicStateMachine.ChangeState(bowstring.thinkState);
            return;
        }

        if (!hasDecideNextCloseAttack)
        {
            ChaseTargetUntilAttackDecision();
        }
        else
        {
            GoToAttackPosition();
        }
    }

    private void GoToAttackPosition()
    {
        bowstring.flipOnVelX = false;

        Vector2 moveDir = (attackPosition - GetItselfPos()).normalized;
        bowstring.SetVelocity(moveDir * bowstring.chaseSpeed);

        if (Vector2.Distance(GetItselfPos(), attackPosition) < 1f)
        {
            EnemyState nextCloseAttackState = nextCloseAttackIsPoke ?
                bowstring.pokeAttackState : bowstring.doubleSlashAttackState;
            logicStateMachine.ChangeState(nextCloseAttackState);
        }
    }

    private void ChaseTargetUntilAttackDecision()
    {
        Vector2 chaseDir = targetHandler.GetDirectionToTarget();
        bowstring.SetVelocity(chaseDir * bowstring.chaseSpeed);

        if (targetHandler.GetDistanceToTarget() < bowstring.makeAttackDecisionDistance)
        {
            hasDecideNextCloseAttack = true;
            nextCloseAttackIsPoke = Random.value > 0.5f;

            attackPosition = targetHandler.GetTargetPosition();

            Vector2 offset = nextCloseAttackIsPoke ? bowstring.pokeOffset : bowstring.doubleSlashOffset;
            offset.x *= bowstring.FacingDir;

            attackPosition += offset;
        }
    }
}

public class BowstringAttackState : BowstringState
{
    private AnimationState animAttackState;
    private int attackIndex;

    public BowstringAttackState(Enemy enemy, AnimationState animAttackState, int attackIndex) : base(enemy)
    {
        this.animAttackState = animAttackState;
        this.attackIndex = attackIndex;
    }

    public override void Enter()
    {
        base.Enter();
        bowstring.StopVelocity();
        attackSet.ChangeAttackType(attackIndex);
        animStateMachine.ChangeState(animAttackState);
    }

    public override void Update()
    {
        base.Update();

        if (bowstring.IsCurrentAnimStateTriggerCalled())
            logicStateMachine.ChangeState(bowstring.thinkState);
    }
}
