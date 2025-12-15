using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class MI12State : EnemyOffensiveState
{
    protected EnemyMI12 mi12;
    public MI12State(Enemy enemy) : base(enemy)
    {
        mi12 = enemy as EnemyMI12;
    }
}

public class MI12ObservationState : MI12State
{
    private float flipInterval = 2;

    public MI12ObservationState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = flipInterval;
        mi12.StopVelocity();
        animStateMachine.ChangeState(mi12.animIdleState);
    }

    public override void Update()
    {
        base.Update();
        if (stateTimer < 0)
        {
            mi12.Flip();
            stateTimer = flipInterval;
        }

        if (!IsTargetValid()) return;

        float targetDistance = targetHandler.GetDistanceToTarget();

        if (targetDistance > mi12.minChargeDistance)
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
    private float chargeDuration;
    private float pushBackForce = 10;
    private bool hasTouchedWallOrEdge;

    public MI12ChargeState(Enemy enemy) : base(enemy)
    {
        chargeDuration = mi12.chargeDuration;
    }

    public override void Enter()
    {
        base.Enter();
        AudioManager.Instance.PlaySFXAt(mi12.chargeSound, mi12.transform.position);

        stateTimer = chargeDuration;
        animStateMachine.ChangeState(mi12.animChargeState);
        hasTouchedWallOrEdge = false;
    }

    public override void Update()
    {
        base.Update();

        if (!hasTouchedWallOrEdge)
        {
            if (mi12.IsGroundEdgeOrWallDetected())
            {
                hasTouchedWallOrEdge = true;
                mi12.StopVelocity();
                mi12.rb.AddForceX(-mi12.FacingDir * pushBackForce, ForceMode2D.Impulse);
            }
        }
        else if (mi12.IsIdle())
        {
            logicStateMachine.ChangeState(mi12.observationState);
        }

        Charging();
    }

    public override void Exit()
    {
        base.Exit();

        mi12.Flip();
    }

    private void Charging()
    {
        if (hasTouchedWallOrEdge) return;

        float t = mi12.chargeSpeedCurve.Evaluate((chargeDuration - stateTimer) / chargeDuration);
        float speed = Mathf.Lerp(0, mi12.maxChargeSpeed, t);

        mi12.SetVelocityX(speed * mi12.FacingDir);
    }
}

public class MI12BlastLaserAttack : MI12State
{
    public MI12BlastLaserAttack(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        mi12.StopVelocity();
        animStateMachine.ChangeState(mi12.animLaserBlastState);
    }

    public override void Update()
    {
        base.Update();

        if (mi12.IsCurrentAnimStateTriggerCalled())
        {
            logicStateMachine.ChangeState(mi12.observationState);
        }
    }
}