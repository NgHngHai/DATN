using UnityEngine;

public class SnatcherOnWallState : SnatcherState
{
    public SnatcherOnWallState(Enemy enemy) : base(enemy)
    {
    }

    public override void Update()
    {
        base.Update();
        if (targetHandler.IsCurrentTargetNotNull())
        {
            logicStateMachine.ChangeState(snatcher.jumpTargetState);
        }
    }

}
