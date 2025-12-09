using UnityEngine;

public class EnemyAnimationEvent : MonoBehaviour
{
    protected Enemy enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    public void CallCurrentAnimationTrigger()
    {
        enemy.CallCurrentAnimationStateTrigger();
    }

    public void TryCurrentAttack()
    {
        enemy.AttackSet.CurrentAttack.TryAttack();
    }
}
