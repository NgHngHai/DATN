using UnityEngine;

/// <summary>
/// Base abstract class for all enemy logic states.
/// Defines shared behavior for entering, updating, and exiting a state.
/// Each specific enemy behavior (e.g., Patrol, Chase, Attack) should inherit from this class.
/// </summary>
public abstract class EnemyState
{
    protected EnemyStateMachine logicStateMachine;
    protected EnemyTargetHandler targetHandler;
    protected Enemy enemy;

    protected float stateTimer;

    public EnemyState(Enemy enemy)
    {
        this.enemy = enemy;
        logicStateMachine = enemy.logicStateMachine;
        targetHandler = enemy.TargetHandler;
    }

    public virtual void Enter() { }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;
    }

    public virtual void FixedUpdate()
    {

    }

    public virtual void Exit() { }

    /// <summary>
    /// Rotates the enemy to face (or face away from) the target horizontally.  
    /// If <paramref name="flipOpposite"/> is true, flips the enemy to face away from the target instead of toward it.
    /// </summary>
    protected void FlipToTarget(bool flipOpposite = false)
    {
        int targetDir = targetHandler.GetHorizontalDirectionToTarget() * (flipOpposite ? -1 : 1);
        if (targetDir != enemy.FacingDir) enemy.Flip();
    }

    protected bool IsTargetValid()
    {
        return targetHandler != null && targetHandler.IsTargetValid();
    }
}
