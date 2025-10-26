using UnityEngine;

public class BoomDashState : BoomState
{
    private Vector2 dashVel;

    public BoomDashState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = boom.dashDuration;
    }

    public override void Update()
    {
        base.Update();

        Dashing();
        ChangeStateOnTargetStatus();

        if (targetHandler.GetDistanceToTarget() < boom.explodeDistance)
        {
            boom.Explode();
        }
    }

    private void ChangeStateOnTargetStatus()
    {
        if (stateTimer > 0) return;

        if (targetHandler.CurrentTarget == null)
        {
            if (targetHandler.GetTargetFacingDir() != boom.facingDir)
                boom.Flip();
            logicStateMachine.ChangeState(boom.targetDetectedState);
        }
        else
        {
            logicStateMachine.ChangeState(boom.patrolState);
        }
    }

    private void Dashing()
    {
        float t = boom.dashCurve.Evaluate(1 - (stateTimer / boom.dashDuration));
        float dashSpeed = Mathf.Lerp(0, boom.maxDashSpeed, t);

        dashVel.x = dashSpeed * boom.facingDir;

        boom.SetVelocity(dashVel);
    }

    public override void Exit()
    {
        base.Exit();
        boom.StopVelocity();
    }
}
