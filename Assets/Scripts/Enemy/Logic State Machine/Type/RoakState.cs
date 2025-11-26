using UnityEngine;

public class RoakState : EnemyState
{
    protected EnemyRoak roak;
    public RoakState(Enemy enemy) : base(enemy)
    {
        roak = enemy as EnemyRoak;
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = roak.createPoisonDelay;
    }

    public override void Update()
    {
        base.Update();

        if (roak.IsIdle())
            animStateMachine.ChangeState(roak.animIdleState);
        else
            animStateMachine.ChangeState(roak.animRunState);

        if (stateTimer < 0)
        {
            stateTimer = roak.createPoisonDelay;
            roak.CreatePoison();
        }
    }
}
