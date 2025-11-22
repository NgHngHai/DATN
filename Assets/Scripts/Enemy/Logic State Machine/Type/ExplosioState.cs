using UnityEngine;

public class ExplosioState : EnemyOffensiveState
{
    protected EnemyExplosio explosio;

    public ExplosioState(Enemy enemy) : base(enemy)
    {
        explosio = enemy as EnemyExplosio;
    }
    protected bool ValidTargetOrObservation()
    {
        if (!IsTargetValid())
        {
            logicStateMachine.ChangeState(explosio.observationState);
            return false;
        }
        return true;
    }
}

public class ExplosioObservationState : ExplosioState
{
    public ExplosioObservationState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        explosio.StopVelocity();
    }

    public override void Update()
    {
        base.Update();
        if (!IsTargetValid()) return;

        bool doBrustFireAttack = targetHandler.GetDistanceToTarget() <= explosio.canBrustFireDistance;

        if(doBrustFireAttack)
        {
            logicStateMachine.ChangeState(explosio.brustFireAttackState);
        }
        else
        {
            logicStateMachine.ChangeState(explosio.stompApproachState);
        }
    }
}

public class ExplosioStompApproachState : ExplosioState
{
    public ExplosioStompApproachState(Enemy enemy) : base(enemy)
    {
    }

    public override void Update()
    {
        base.Update();
        if (!ValidTargetOrObservation()) return;

        bool closeEnoughToStomp = targetHandler.GetDistanceToTarget() <= explosio.stompDistance;
        if (closeEnoughToStomp)
            logicStateMachine.ChangeState(explosio.stompAttackState);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (!ValidTargetOrObservation()) return;

        Vector2 moveDir = targetHandler.GetDirectionToTarget();
        explosio.SetVelocity(explosio.flySpeed * moveDir);
    }

}

public class ExplosioStompAttackState : ExplosioState
{
    private Vector2 startPos;
    private Vector2 targetPos;
    private float elapsed;

    public ExplosioStompAttackState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        base.Enter();
        if (!ValidTargetOrObservation()) return;

        explosio.StopVelocity();
        ToggleStompHurtBox(true);
        FlipToTarget();

        startPos = explosio.transform.position;
        targetPos = targetHandler.CurrentTarget.position;
        elapsed = 0f;
    }

    public override void Update()
    {
        base.Update();

        elapsed += Time.deltaTime;
        float t = elapsed / explosio.stompDuration;

        float height = explosio.maxStompHeight * explosio.stompHeightCurve.Evaluate(t);
        Vector2 nextPos = Vector2.Lerp(startPos, targetPos, t);
        nextPos.y += height;

        explosio.transform.position = nextPos;

        if (t >= 1f)
        {
            logicStateMachine.ChangeState(explosio.restState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        explosio.StopVelocity();
        ToggleStompHurtBox(false);
    }

    private void ToggleStompHurtBox(bool enable)
    {
        explosio.stompHurtBox.ToggleHurtCollider(enable);
    }
}


public class ExplosioBrustFireAttackState : ExplosioState
{
    private Vector2 directionToTarget;
    private int fireCount;

    public ExplosioBrustFireAttackState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        if (!ValidTargetOrObservation()) return;

        explosio.StopVelocity();
        FlipToTarget();
        fireCount = 0;
        directionToTarget = targetHandler.GetDirectionToTarget();
    }

    public override void Update()
    {
        base.Update();

        if(fireCount <= explosio.maxFireCount)
        {
            BrustFire();
        }
        else
        {
            logicStateMachine.ChangeState(explosio.restState);
        }
    }

    private void BrustFire()
    {
        if (stateTimer > 0) return;

        float baseAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x);
        float randomAngle = baseAngle + Random.Range(-explosio.fireAngle, explosio.fireAngle) * Mathf.Deg2Rad;
        Vector2 fireDirection = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));

        attackSet.CurrentAttack.AttackPointLookAtDirection(fireDirection);
        TryCurrentAttack();

        stateTimer = explosio.fireRate;
        fireCount++;
    }
}

public class ExplosioRestState : ExplosioState
{
    public ExplosioRestState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = explosio.restTime;
        explosio.StopVelocity();
    }

    public override void Update()
    {
        base.Update();
        if(stateTimer < 0)
        {
            logicStateMachine.ChangeState(explosio.observationState);
        }
    }
}