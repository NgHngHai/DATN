using UnityEngine;

public class EnemyPatrolGroundEdgeWall : EnemyPatrolGround
{
    protected override void Patrolling()
    {
        if (!groundEnemy.IsGrounded()) return;

        groundEnemy.SetVelocityX(patrolDirX * patrolSpeed);

        if (groundEnemy.IsGroundEdgeOrWallDetected())
        {
            EnterRest();
        }
    }
}
