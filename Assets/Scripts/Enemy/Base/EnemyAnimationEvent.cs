using UnityEngine;

public class EnemyAnimationEvent : EntityAnimationEvent
{
    protected Enemy enemy;

    private void Start()
    {
        enemy = entity as Enemy;
    }

    public void TryCurrentAttack()
    {
        enemy.AttackSet.CurrentAttack.TryAttack();
    }
}
