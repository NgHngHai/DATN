using UnityEngine;

/// <summary>
/// Handles ranged enemy attacks using bullet projectiles.
/// Fires bullets from the attack point and checks for line-of-sight to the target.
/// </summary>
public class EnemyRangedAttack : EnemyAttackBehavior
{
    [Header("Ranged Attack")]
    [SerializeField] protected LayerMask whatIsObstacle;
    [Header("Bullet")]
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected float bulletSpeed;
    [SerializeField] protected bool looksAtFireDirection;

    public override void TryAttack()
    {
        Quaternion lookRot = looksAtFireDirection
            ? Quaternion.FromToRotation(Vector3.up, attackPoint.right)
            : Quaternion.identity;

        GameObject bulletObject = Instantiate(bulletPrefab, attackPoint.position, lookRot);

        Bullet bullet = bulletObject.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.SetVelocity(attackPoint.right * bulletSpeed);
        }
    }

    public override bool IsTargetInAttackArea(Transform target)
    {
        if (target == null) return false;

        Vector2 dirToTarget = (target.position - attackPoint.position).normalized;
        float distance = Vector2.Distance(attackPoint.position, target.position);
        RaycastHit2D hit = Physics2D.Raycast(attackPoint.position, dirToTarget, distance, whatIsObstacle);
        bool unobstructed = hit.collider == null;

        return unobstructed;
    }
}
