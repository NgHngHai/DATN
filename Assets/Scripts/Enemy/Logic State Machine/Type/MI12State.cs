using UnityEngine;

public class MI12State : EnemyOffensiveState
{
    protected EnemyMI12 mi12;
    public MI12State(Enemy enemy) : base(enemy)
    {
        mi12 = enemy as EnemyMI12;
    }
}

public class MI12RestState : MI12State
{
    public MI12RestState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = mi12.chargeRestTime;
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer > 0 || !IsTargetValid()) return;

        float toTargetDistance = targetHandler.GetDistanceToTarget();

        if(toTargetDistance > mi12.decideToChargeDistance)
        {
            logicStateMachine.ChangeState(mi12.chargeState);
        }
        else
        {
            logicStateMachine.ChangeState(mi12.blastLaserAttackState);
        }

    }
}

public class MI12ChargeState : MI12State
{
    private float reachMaxSpeedTime;
    public MI12ChargeState(Enemy enemy) : base(enemy)
    {
        reachMaxSpeedTime = mi12.reachMaxSpeedTime;
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = reachMaxSpeedTime;
        FlipToTarget();
    }

    public override void Update()
    {
        base.Update();

        if (mi12.IsGroundEdgeOrWallDetected())
        {
            logicStateMachine.ChangeState(mi12.restState);
        }

        float t = mi12.chargeSpeedCurve.Evaluate((reachMaxSpeedTime - stateTimer) / reachMaxSpeedTime);
        float speed = Mathf.Lerp(0, mi12.maxChargeSpeed, t);

        Vector2 chargeVel = new Vector2(speed * mi12.FacingDir, 0);
        
        mi12.SetVelocity(chargeVel);
    }
}

public class MI12BlastLaserAttack : MI12State
{
    public MI12BlastLaserAttack(Enemy enemy) : base(enemy)
    {
    }
}