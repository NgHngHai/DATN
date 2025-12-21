using UnityEngine;

/// <summary>
/// Detects targets in a straight line based on the entity's facing direction.
/// Stops detection if an obstacle blocks the line of sight.
/// </summary>
public class EnemyLineDetector : EnemyTargetDetector
{
    protected override Transform GetFirstDetectedTarget()
    {
        Vector2 dir = GetFaceDirection();
        RaycastHit2D hit = Physics2D.Raycast(detectPoint.position, dir, detectDistance, targetMask | obstacleMask);

        if (!hit) return null;
        if(Utility.IsGameObjectInLayer(hit.collider.gameObject, obstacleMask)) return null;
        if (Utility.IsGameObjectInLayer(hit.collider.gameObject, targetMask)) return hit.collider.transform;

        return null;
    }

    protected override void DrawGizmos()
    {
        base.DrawGizmos();

        Vector2 dir = GetFaceDirection();
        Gizmos.DrawLine(detectPoint.position, detectPoint.position + (Vector3)dir * detectDistance);
    }

    private Vector2 GetFaceDirection()
    {
        if (enemy == null) return Vector2.right;
        return new Vector2(enemy.FacingDir, 0);
    }
}
