using UnityEngine;

/// <summary>
/// Controls the logical state transitions of the entity.
/// Manages entering, updating, and exiting states within the entity’s finite state machine (FSM).
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
        //Debug.Log($"[Logic] {currentState.GetType().Name} -> {newState.GetType().Name}");

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