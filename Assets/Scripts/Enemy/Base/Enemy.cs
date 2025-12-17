using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Base class for all enemies.
/// Handles movement, facing direction, and state machine updates.
/// </summary>
public abstract class Enemy : Entity
{
    [Header("Enemy")]
    [SerializeField] protected GameObject deathExplosion;
    public bool flipOnVelX = true;
    public EnemyStateMachine logicStateMachine;
    protected EnemyAttackSet attackSet;
    protected EnemyTargetHandler targetHandler;

    protected EffectEvents effectEvents;
    protected Health health;

    protected override void Awake()
    {
        base.Awake();

        health = GetComponent<Health>();
        effectEvents = GetComponent<EffectEvents>();
        attackSet = GetComponent<EnemyAttackSet>();
        targetHandler = GetComponent<EnemyTargetHandler>();
        logicStateMachine = new EnemyStateMachine();
    }

    protected virtual void Start()
    {
    }

    private void OnEnable()
    {
        if (health == null) return;
        health.OnDamagedWithReaction.AddListener(OnDamageTaken);
        health.OnDeath.AddListener(OnDeath);
    }

    private void OnDisable()
    {
        if (health == null) return;
        health.OnDamagedWithReaction.RemoveListener(OnDamageTaken);
        health.OnDeath.RemoveListener(OnDeath);
    }

    protected override void Update()
    {
        base.Update();

        if (flipOnVelX)
        {
            FlipOnVelocityX();
        }
        logicStateMachine.UpdateCurrentState();
    }

    protected virtual void FixedUpdate()
    {
        logicStateMachine.FixedUpdateCurrentState();
    }

    protected virtual void OnDamageTaken(int appliedAmount, Vector2 hitDir, 
        bool shouldTriggerHitReaction)
    {
        effectEvents?.InvokeDamagedWithReaction(hitDir);
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
        CreateDeathExplosionAt(transform.position);
        Destroy(gameObject);
    }

    protected void CreateDeathExplosionAt(Vector2 position)
    {
        if (deathExplosion != null)
            Instantiate(deathExplosion, position, transform.rotation);
    }

    public Health GetHealth() => health;
    public EnemyAttackSet AttackSet => attackSet;
    public EnemyTargetHandler TargetHandler => targetHandler;
}
