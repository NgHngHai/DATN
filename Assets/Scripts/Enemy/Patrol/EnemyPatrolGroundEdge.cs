using UnityEngine;

/// <summary>
/// Extends <see cref="EnemyPatrolBehavior"/> with ground patrol logic.
/// Allows the entity to patrol left and right, turning around when reaching an edge or wall.
/// </summary>
public class EnemyPatrolGroundEdge : EnemyPatrolBehavior
{
    GroundEnemy groundEnemy;
    Vector2 patrolDirection;

    protected override void Awake()
    {
        base.Awake();
        groundEnemy = enemy as GroundEnemy;
    }

    protected override void EnterPatrol()
    {
        base.EnterPatrol();

        if (groundEnemy.IsGroundEdgeOrWallDetected())
        {
            patrolDirection.x = groundEnemy.FacingDir * -1;
            groundEnemy.Flip();
        }
        else
        {
            patrolDirection.x = Random.Range(0, 2) == 1 ? 1 : -1;
            if (patrolDirection.x != groundEnemy.FacingDir) groundEnemy.Flip();
        }
    }

    protected override void Patrolling()
    {
        if (!groundEnemy.IsGrounded()) return;

        groundEnemy.SetVelocity(patrolDirection * patrolSpeed);

        if (groundEnemy.IsGroundEdgeOrWallDetected())
        {
            EnterRest();
        }
    }
}
