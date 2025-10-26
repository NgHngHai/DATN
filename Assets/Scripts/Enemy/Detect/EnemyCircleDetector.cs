using UnityEngine;

/// <summary>
/// Detects targets within a circular radius around the detect point.
/// </summary>
public class EnemyCircleDetector : EnemyTargetDetector
{
    protected override Transform GetFirstDetectedTarget()
    {
        Collider2D hitTarget = Physics2D.OverlapCircle(
            detectPoint.position,
            detectDistance,
            whatIsTarget
        );

        if (hitTarget != null) return hitTarget.transform;
        return null;
    }

    protected override void OnDrawGizmos()
    {
        if (!drawGizmos || detectPoint == null) return;
        base.OnDrawGizmos();

        Gizmos.DrawWireSphere(detectPoint.position, detectDistance);
    }
}
