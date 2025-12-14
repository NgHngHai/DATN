using UnityEngine;

public abstract class EnemyPatrolGround : EnemyPatrolBehavior
{
    protected GroundEnemy groundEnemy;
    protected int patrolDirX = 1;

    protected override void Awake()
    {
        base.Awake();
        groundEnemy = enemy as GroundEnemy;
    }

    protected override void EnterPatrol()
    {
        base.EnterPatrol();

        FlipIfTouchWallOrGroundEdge();

        if(!groundEnemy.IsGroundEdgeOrWallDetected())
        {
            patrolDirX = Random.Range(0, 2) == 1 ? 1 : -1;
            if (patrolDirX != groundEnemy.FacingDir) groundEnemy.Flip();
        }
    }

    protected void FlipIfTouchWallOrGroundEdge()
    {
        if (groundEnemy.IsGroundEdgeOrWallDetected())
        {
            patrolDirX = groundEnemy.FacingDir * -1;
            groundEnemy.Flip();
        }
    }
}
