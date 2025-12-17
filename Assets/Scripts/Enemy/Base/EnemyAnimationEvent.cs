using UnityEngine;

public class EnemyAnimationEvent : EntityAnimationEvent
{
    protected Enemy enemy;

    protected override void Awake()
    {
        base.Awake();
        enemy = entity as Enemy;
    }

    public void TryCurrentAttack()
    {
        enemy.AttackSet.CurrentAttack.TryAttack();
    }
}
