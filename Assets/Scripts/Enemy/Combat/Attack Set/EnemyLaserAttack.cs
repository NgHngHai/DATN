using UnityEngine;

public class EnemyLaserAttack : EnemyAttackBehavior
{
    [Header("Attack: Laser")]
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float laserLength = 20f;
    [SerializeField] private float laserDuration = 0.2f;

    protected override void Attack()
    {
        GameObject laserObj = Instantiate(laserPrefab, attackPoint.position, Quaternion.identity);

        LaserBeam laser = laserObj.GetComponent<LaserBeam>();
        if (laser != null)
        {
            laser.Fire(
                attackPoint.position,
                attackPoint.right,
                laserLength,
                laserDuration
            );
        }
    }

    public override bool IsTargetInAttackArea(Transform target)
    {
        if (target == null) return false;

        Vector2 start = attackPoint.position;
        Vector2 dir = (target.position - attackPoint.position).normalized;
        float distance = Vector2.Distance(start, target.position);

        if (distance > laserLength)
            return false;

        // laser không bắn xuyên tường → check obstacle
        RaycastHit2D hit = Physics2D.Raycast(start, dir, distance, obstacleMask);
        return hit.collider == null;
    }
}
