using UnityEngine;

public class SnatcherAttackState : SnatcherCombatState
{
    public SnatcherAttackState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        attackBehavior.TryAttack();
        logicStateMachine.ChangeState(snatcher.chaseState);
    }

    protected override void UpdateIfTargetNotNull()
    {
    }
}
