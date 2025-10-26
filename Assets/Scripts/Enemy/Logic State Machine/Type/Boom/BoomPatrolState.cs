using UnityEngine;

public class BoomPatrolState : BoomState
{
    protected EnemyPatrolBehavior patrolBehavior;
    public BoomPatrolState(Enemy enemy) : base(enemy)
    {
        patrolBehavior = boom.GetComponent<EnemyPatrolBehavior>();
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
            logicStateMachine.ChangeState(boom.targetDetectedState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        patrolBehavior.StopPatrolProcess();
    }
}
