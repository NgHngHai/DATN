
public class Fly0SleepState : Fly0State
{
    bool isSleeping = true;

    public Fly0SleepState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = 1;
    }

    public override void Update()
    {
        if (isSleeping)
        {
            if (targetHandler.IsCurrentTargetNotNull())
                isSleeping = false;
        }
        else
        {
            base.Update();
            if (stateTimer < 0)
                logicStateMachine.ChangeState(fly0.chaseState);
        }
    }
}
