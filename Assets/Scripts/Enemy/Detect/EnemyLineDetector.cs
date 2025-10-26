using UnityEngine;

/// <summary>
/// Detects targets in a straight line based on the enemy's facing direction.
/// Stops detection if an obstacle blocks the line of sight.
/// </summary>
public class EnemyLineDetector : EnemyTargetDetector
{
    protected override Transform GetFirstDetectedTarget()
    {
        Vector2 dir = GetFaceDirection();
        RaycastHit2D hit = Physics2D.Raycast(detectPoint.position, dir, detectDistance, whatIsTarget | whatIsObstacle);

        if (!hit) return null;
        if(PhysicsUtils.IsGameObjectInLayer(hit.collider.gameObject, whatIsObstacle)) return null;
        if (PhysicsUtils.IsGameObjectInLayer(hit.collider.gameObject, whatIsTarget)) return hit.collider.transform;

        return null;
    }

    protected override void OnDrawGizmos()
    {
        if (!drawGizmos || detectPoint == null) return;

        base.OnDrawGizmos();

        Vector2 dir = GetFaceDirection();
        Gizmos.DrawLine(detectPoint.position, detectPoint.position + (Vector3)dir * detectDistance);
    }
}
