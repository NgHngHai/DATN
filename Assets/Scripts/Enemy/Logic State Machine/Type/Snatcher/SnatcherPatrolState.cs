using UnityEngine;

public class SnatcherPatrolState : SnatcherState
{
    EnemyPatrolBehavior patrolBehavior;
    public SnatcherPatrolState(Enemy enemy) : base(enemy)
    {
        patrolBehavior = snatcher.GetComponent<EnemyPatrolBehavior>();
    }

    public override void Enter()
    {
        base.Enter();
        patrolBehavior.StartPatrolProcess();
    }

    public override void Update()
    {
        base.Update();
        if (targetHandler.IsCurrentTargetNotNull())
        {
            logicStateMachine.ChangeState(snatcher.chaseState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        patrolBehavior.StopPatrolProcess();
    }
}
