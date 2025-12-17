using UnityEngine;

public class EnemyAnimationEvent : EntityAnimationEvent
{
    protected Enemy enemy;

    protected virtual void Start()
    {
        enemy = entity as Enemy;
    }

    public void TryCurrentAttack()
    {
        enemy.AttackSet.CurrentAttack.TryAttack();
    }
}
