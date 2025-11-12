
/// <summary>
/// Controls the logical state transitions of the enemy.
/// Manages entering, updating, and exiting states within the enemy’s finite state machine (FSM).
/// </summary>
public class EnemyStateMachine
{
    public EnemyState currentState;

    public void Initialize(EnemyState startState)
    {
        currentState = startState;
        currentState.Enter();
    }

    public void ChangeState(EnemyState newState)
    {
        if (newState == null) return;

        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void UpdateCurrentState()
    {
        currentState?.Update();
    }

    public void FixedUpdateCurrentState()
    {
        currentState?.FixedUpdate();
    }
}