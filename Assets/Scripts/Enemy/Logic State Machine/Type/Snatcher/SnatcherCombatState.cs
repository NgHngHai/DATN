using UnityEngine;

public abstract class SnatcherCombatState : SnatcherState
{
    protected EnemyAttackBehavior attackBehavior;

    public SnatcherCombatState(Enemy enemy) : base(enemy)
    {
        attackBehavior = snatcher.GetComponent<EnemyAttackBehavior>();
    }

    public override void Update()
    {
        base.Update();
        if (targetHandler.IsCurrentTargetNotNull())
        {
            UpdateIfTargetNotNull();
        }
        else
        {
            logicStateMachine.ChangeState(snatcher.patrolState);
        }
    }

    protected abstract void UpdateIfTargetNotNull();
}
