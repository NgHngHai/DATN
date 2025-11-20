using UnityEngine;

public class EnemyDolRE : Enemy
{
    public DolREIdleObservationState idleObservationState;
    public DolREExplosionProjectileState explosionProjectileState;
    public DolREHomingProjectileState homingProjectileState;

    protected override void Awake()
    {
        base.Awake();
        idleObservationState = new DolREIdleObservationState(this);
        explosionProjectileState = new DolREExplosionProjectileState(this);
        homingProjectileState = new DolREHomingProjectileState(this);
    }

    protected override void Start()
    {
        base.Start();
        logicStateMachine.Initialize(idleObservationState);
    }
}