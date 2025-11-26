using UnityEngine;

/// <summary>
/// Base class for all enemies.
/// Handles movement, facing direction, and state machine updates.
/// </summary>
public abstract class Enemy : Entity
{
    public EnemyStateMachine logicStateMachine;
    protected EnemyAttackSet attackSet;
    protected EnemyTargetHandler targetHandler;

    protected override void Awake()
    {
        base.Awake();
        attackSet = GetComponent<EnemyAttackSet>();
        targetHandler = GetComponent<EnemyTargetHandler>();
        logicStateMachine = new EnemyStateMachine();
    }

    protected virtual void Start()
    {
    }

    protected override void Update()
    {
        base.Update();
        FlipOnVelocityX();
        logicStateMachine.UpdateCurrentState();
    }


    protected virtual void FixedUpdate()
    {
        logicStateMachine.FixedUpdateCurrentState();
    }

    public virtual void SetVelocity(Vector2 velocity)
    {
        rb.linearVelocity = velocity;
    }

    public void StopVelocity()
    {
        rb.linearVelocity = Vector2.zero;
    }

    public bool IsIdle() => rb.linearVelocity.magnitude < 0.1f;

    public virtual void OnDeath()
    {
        Destroy(gameObject);
    }

    public EnemyAttackSet AttackSet => attackSet;
    public EnemyTargetHandler TargetHandler => targetHandler;
}
