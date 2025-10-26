using UnityEngine;

public class Fly0AttackState : Fly0State
{
    public Fly0AttackState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        attackBehavior.AttackPointLookAt(targetHandler.GetDirectionToTarget());
        attackBehavior.TryAttack();
        logicStateMachine.ChangeState(fly0.hoverAroundState);
    }
}

