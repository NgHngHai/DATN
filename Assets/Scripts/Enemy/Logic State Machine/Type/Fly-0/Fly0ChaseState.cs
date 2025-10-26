using UnityEngine;

public class Fly0ChaseState : Fly0State
{
    public Fly0ChaseState(Enemy enemy) : base(enemy)
    {
    }

    public override void Update()
    {
        base.Update();

        Vector2 chaseVel = targetHandler.GetDirectionToTarget() * fly0.chaseSpeed;
        fly0.SetVelocity(chaseVel);

        if(targetHandler.GetDistanceToTarget() < fly0.hoverAroundDistance)
        {
            logicStateMachine.ChangeState(fly0.hoverAroundState);
        }
    }
}
