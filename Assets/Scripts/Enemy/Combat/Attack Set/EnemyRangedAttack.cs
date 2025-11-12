using UnityEngine;

/// <summary>
/// Handles ranged enemy attacks using projectile projectiles.
/// Fires bullets from the attack point and checks for line-of-sight to the target.
/// </summary>

[DisallowMultipleComponent]
public class EnemyRangedAttack : EnemyAttackBehavior
{
    [Header("Attack: Ranged")]
    [SerializeField] protected LayerMask obstacleMask;
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected float projectileSpeed;
    [SerializeField] protected bool looksAtFireDirection;

    protected override void Attack()
    {
        Quaternion lookRot = looksAtFireDirection
            ? Quaternion.FromToRotation(Vector3.up, attackPoint.right)
            : Quaternion.identity;

        GameObject newProjectileObject = Instantiate(projectilePrefab, attackPoint.position, lookRot);

        Projectile projectile = newProjectileObject.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.SetVelocity(attackPoint.right * projectileSpeed);
        }
    }

    public override bool IsTargetInAttackArea(Transform target)
    {
        if (target == null) return false;

        Vector2 dirToTarget = (target.position - attackPoint.position).normalized;
        float distance = Vector2.Distance(attackPoint.position, target.position);
        RaycastHit2D hit = Physics2D.Raycast(attackPoint.position, dirToTarget, distance, obstacleMask);
        bool unobstructed = hit.collider == null;

        return unobstructed;
    }
}
