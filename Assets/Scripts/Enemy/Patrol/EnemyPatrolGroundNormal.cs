using UnityEngine;

public class EnemyPatrolGroundNormal : EnemyPatrolGround
{
    [Tooltip("Enter rest after patrolling for a distance.")]
    [SerializeField] private Vector2 stopDistanceLimit;

    private Vector2 lastPos;
    private float totalDistance;
    private float willStopDistance;

    protected override void EnterPatrol()
    {
        base.EnterPatrol();

        totalDistance = 0;
        lastPos = transform.position;
        willStopDistance = Random.Range(stopDistanceLimit.x, stopDistanceLimit.y);
    }

    protected override void Patrolling()
    {
        if (!groundEnemy.IsGrounded()) return;

        groundEnemy.SetVelocityX(patrolDirX * patrolSpeed);

        Vector2 currentPos = transform.position;
        totalDistance += Vector2.Distance(lastPos, currentPos);
        lastPos = currentPos;

        FlipIfTouchWallOrGroundEdge();

        if (totalDistance > willStopDistance)
            EnterRest();
    }
}
