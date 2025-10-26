using UnityEngine;

public class RoakState : EnemyState
{
    protected Roak roak;
    public RoakState(Enemy enemy) : base(enemy)
    {
        roak = enemy as Roak;
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = roak.createPoisonDelay;
    }

    public override void Update()
    {
        base.Update();
        if(stateTimer < 0)
        {
            stateTimer = roak.createPoisonDelay;
            roak.CreatePoison();
        }
    }
}
