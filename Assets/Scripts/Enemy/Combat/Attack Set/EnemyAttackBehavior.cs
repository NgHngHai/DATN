using UnityEngine;

/// <summary>
/// Base class for enemy attack behaviors.
/// Defines common attack logic and target checking interface.
/// </summary>

[RequireComponent(typeof(EnemyAttackSet))]
public abstract class EnemyAttackBehavior : MonoBehaviour
{
    [Header("Base Properties")]
    [SerializeField] protected Transform attackPoint;
    [SerializeField] protected float attackCooldown = 1f;

    private float lastAttackTime;

    protected abstract void Attack();
    public abstract bool IsTargetInAttackArea(Transform target);

    public void TryAttack()
    {
        if (!IsReadyToAttack()) return;

        lastAttackTime = Time.time;
        Attack();
    }

    public bool IsReadyToAttack()
    {
        return Time.time >= lastAttackTime + attackCooldown;
    }

    public void AttackPointLookAtDirection(Vector3 lookDir)
    {
        attackPoint.right = lookDir;
    }

    public void AttackPointLookAt(Transform target)
    {
        if (target == null) return;
        Vector3 lookDir = (target.position - attackPoint.position).normalized;
        AttackPointLookAtDirection(lookDir);
    }
}
