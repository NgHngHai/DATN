using UnityEngine;

public class RoakState : EnemyOffensiveState
{
    protected EnemyRoak roak;
    public RoakState(Enemy enemy) : base(enemy)
    {
        roak = enemy as EnemyRoak;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        if (roak.IsIdle())
            animStateMachine.ChangeState(roak.animIdleState);
        else
            animStateMachine.ChangeState(roak.animRunState);

        if (IsCurrentAttackReady())
        {
            TryCurrentAttack();
        }
    }
}
