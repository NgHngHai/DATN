using UnityEngine;

/// <summary>
/// Handles close-range enemy attacks.
/// Detects and damages targets within a circular area around the attack point.
/// </summary>
public class EnemyMeleeAttack : EnemyAttackBehavior
{
    [Header("Melee Attack")]
    [SerializeField] protected LayerMask whatIsTarget;
    [SerializeField] protected float attackRadius = 1f;
    [SerializeField] protected bool drawGizmos = true;
    [SerializeField] protected int meleeDamage;

    public override void TryAttack()
    {
        Vector2 center = attackPoint.position;

        Collider2D[] hits = Physics2D.OverlapCircleAll(center, attackRadius, whatIsTarget);

        foreach (var hit in hits)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();

            if (damageable != null)
                damageable.TakeDamage(meleeDamage);
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
