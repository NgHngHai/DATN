using UnityEngine;

/// <summary>
/// Handles close-range enemy attacks.
/// Detects and damages targets within a circular area around the attack point.
/// </summary>

[DisallowMultipleComponent]
public class EnemyMeleeAttack : EnemyAttackBehavior
{
    [Header("Attack: Melee")]
    [SerializeField] protected float attackRadius = 1f;
    [SerializeField] protected bool drawGizmos = true;

    protected override void Attack()
    {
        Vector2 center = attackPoint.position;

        Collider2D[] hits = Physics2D.OverlapCircleAll(center, attackRadius, damageMask);

        foreach (var hit in hits)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();

            if (damageable != null)
                damageable.TakeDamage(attackDamage);
        }
    }

    public override bool IsTargetInAttackArea(Transform target)
    {
        Collider2D col = target?.GetComponent<Collider2D>();
        if (col == null) return false;
        return Vector2.Distance(attackPoint.position, col.bounds.center) <= attackRadius;
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos || attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
