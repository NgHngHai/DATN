using UnityEngine;

public class EnemyOffensiveState : EnemyState
{
    protected EnemyAttackSet attackSet;

    public EnemyOffensiveState(Enemy enemy) : base(enemy)
    {
        attackSet = enemy.AttackSet;
    }

    protected void TryCurrentAttack()
    {
        attackSet?.TryCurrentAttack();
    }

    protected bool IsCurrentAttackReady()
    {
        return attackSet != null && attackSet.IsCurrentAttackReady();
    }

    protected bool IsTargetInCurrentAttackArea(Transform target)
    {
        return attackSet != null && attackSet.IsTargetInCurrentAttackArea(target);
    }
}
