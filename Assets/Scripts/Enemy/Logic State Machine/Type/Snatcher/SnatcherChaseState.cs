using UnityEngine;

public class SnatcherChaseState : SnatcherCombatState
{
    Vector2 chaseVel;
    public SnatcherChaseState(Enemy enemy) : base(enemy)
    {
    }

    protected override void UpdateIfTargetNotNull()
    {
        int targetDirX = targetHandler.GetDirectionToTarget().x > 0 ? 1 : -1;

        chaseVel.x = snatcher.moveSpeed * targetDirX;

        if (snatcher.facingDir != targetDirX)
            snatcher.Flip();

        snatcher.SetVelocity(chaseVel);

        if (attackBehavior.IsTargetInAttackArea(targetHandler.CurrentTarget))
        {
            snatcher.StopVelocity();

            if (stateTimer > 0) return;

            stateTimer = snatcher.attackCooldown;
            logicStateMachine.ChangeState(snatcher.attackState);
        }
    }
}
