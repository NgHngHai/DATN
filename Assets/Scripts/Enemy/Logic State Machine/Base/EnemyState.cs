using UnityEngine;

/// <summary>
/// Base abstract class for all enemy logic states.
/// Defines shared behavior for entering, updating, and exiting a state.
/// Each specific enemy behavior (e.g., Patrol, Chase, Attack) should inherit from this class.
/// </summary>
public abstract class EnemyState
{
    protected EnemyStateMachine logicStateMachine;
    protected Enemy enemy;

    protected float stateTimer;

    public EnemyState(Enemy enemy)
    {
        this.enemy = enemy;
        logicStateMachine = enemy.logicStateMachine;
    }

    public virtual void Enter() { }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;
    }

    public virtual void Exit() { }
}
