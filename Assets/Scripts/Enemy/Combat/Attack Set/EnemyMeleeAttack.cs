using System.Collections;
using UnityEngine;

/// <summary>
/// An enemy attack deals damage at close range using <see cref="HurtBox"/>.
/// </summary>
public class EnemyMeleeAttack : EnemyAttackBehavior
{
    [Header("Attack: Melee")]
    [SerializeField] private HurtBox hurtBox;
    [SerializeField] private float activeDuration = 0.2f;

    private Coroutine attackRoutine;
    private float hurtBoxRadius;

    protected void Start()
    {
        hurtBoxRadius = hurtBox.GetComponent<Collider2D>().bounds.extents.magnitude;
        hurtBox.ToggleHurtCollider(false);
    }

    protected override void Attack()
    {
        if (attackRoutine != null)
            StopCoroutine(attackRoutine);

        attackRoutine = StartCoroutine(DoAttack());
    }

    private IEnumerator DoAttack()
    {
        hurtBox.ToggleHurtCollider(true);

        yield return new WaitForSeconds(activeDuration);

        hurtBox.ToggleHurtCollider(false);
    }

    public override bool IsTargetInAttackArea(Transform target)
    {
        if (target == null) return false;
        return Vector2.Distance(attackPoint.position, target.position) <= hurtBoxRadius;
    }
}
