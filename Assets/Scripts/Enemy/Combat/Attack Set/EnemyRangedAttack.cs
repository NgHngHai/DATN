using UnityEngine;

/// <summary>
/// An enemy attack that fires a <see cref="Projectile"/> to the right of the attack point.
/// </summary>
public class EnemyRangedAttack : EnemyAttackBehavior
{
    [Header("Attack: Ranged")]
    [SerializeField] protected LayerMask obstacleMask;
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected float projectileSpeed;
    [SerializeField] protected float canAttackDistance = 5;
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

        return unobstructed && distance < canAttackDistance;
    }
}
