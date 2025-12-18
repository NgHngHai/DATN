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
        if (!IsCurrentAttackValid()) return;
        attackSet.CurrentAttack.TryAttack();
    }

    protected bool IsCurrentAttackReady()
    {
        if (!IsCurrentAttackValid()) return false;
        return attackSet.CurrentAttack.IsReadyToAttack();
    }

    protected bool IsCurrentTargetInAttackArea()
    {
        if (!IsCurrentAttackValid() || !IsTargetValid()) return false;
        return attackSet.CurrentAttack.IsTargetInAttackArea(targetHandler.CurrentTarget);
    }

    private bool IsCurrentAttackValid() => attackSet != null && attackSet.CurrentAttack != null;
}
