using UnityEngine;

public class BoomTargetDetectedState : BoomState
{
    private float jumpForce = 2f;
    private float waitTime = 1f;

    public BoomTargetDetectedState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        boom.SetVelocity(new Vector2(0, jumpForce));
        stateTimer = waitTime;
    }

    public override void Update()
    {
        base.Update();

        if(targetHandler.CurrentTarget == null)
        {
            logicStateMachine.ChangeState(boom.patrolState);
            return;
        }

        if(boom.facingDir != targetHandler.GetTargetFacingDir())
        {
            boom.Flip();
        }

        if(stateTimer < 0)
        {
            logicStateMachine.ChangeState(boom.dashState);
        }
    }

}
