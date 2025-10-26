using UnityEngine;

/// <summary>
/// Base class for enemy attack behaviors.
/// Defines common attack logic and target checking interface.
/// </summary>
public abstract class EnemyAttackBehavior : MonoBehaviour
{
    [SerializeField] protected Transform attackPoint;

    public void AttackPointLookAt(Vector3 lookDir)
    {
        attackPoint.right = lookDir;
    }

    public abstract void TryAttack();
    public virtual bool IsTargetInAttackArea(Transform target) { return false; }
}
