using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemyLaserAttack : EnemyAttackBehavior
{
    [Header("Attack: Laser Beam")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float laserLength = 20f;
    [SerializeField] private float laserDuration = 0.1f;
    [SerializeField] private float canAttackDistance = 5f;

    private float initialLaserWidth;

    private void Awake()
    {
        if (lineRenderer == null) return;
        initialLaserWidth = lineRenderer.startWidth;
    }

    protected override void Attack()
    {
        Vector3 start = attackPoint.position;
        Vector3 dir = attackPoint.right;

        Vector3 end = CalculateLaserEndPoint(start, dir);

        ShowLaser(start, end);

        StartCoroutine(ShrinkLaserWidthOverTime());
    }

    public override bool IsTargetInAttackArea(Transform target)
    {
        if (target == null) return false;

        Vector2 dirToTarget = (target.position - attackPoint.position).normalized;
        float distance = Vector2.Distance(attackPoint.position, target.position);

        if (distance > canAttackDistance) return false;

        bool hitObstacle = PhysicsUtils.IsRaycastHit(attackPoint.position, dirToTarget, distance, obstacleMask);
        return !hitObstacle;
    }

    private Vector3 CalculateLaserEndPoint(Vector3 start, Vector3 dir)
    {
        Vector3 end = start + dir * laserLength;

        RaycastHit2D hit = Physics2D.Raycast(start, dir, laserLength, damageMask);
        if (hit.collider != null)
        {
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
                damageable.TakeDamage(attackDamage);
        }

        hit = Physics2D.Raycast(start, dir, laserLength, obstacleMask);
        if (hit.collider != null)
        {
            end = hit.point;
        }

        return end;
    }

    private void ShowLaser(Vector3 start, Vector3 end)
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.startWidth = initialLaserWidth;
        lineRenderer.endWidth = initialLaserWidth;
    }

    // Consider using Animator Controller in the future
    private IEnumerator ShrinkLaserWidthOverTime()
    {
        float elapsed = 0f;

        while (elapsed < laserDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / laserDuration;

            float width = Mathf.Lerp(initialLaserWidth, 0f, t);
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;

            yield return null;
        }
        lineRenderer.enabled = false;
    }
}