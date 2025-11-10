using System;
using UnityEngine;

/// <summary>
/// Special ground enemy that clings to walls, jumps onto targets, chases, and uses melee attack.
/// If it loses sight of target, it will go back to patrol like a normal ground enemy.
/// </summary>
public class EnemySnatcher : GroundEnemy
{
    [Header("Enemy: Snatcher")]
    [Tooltip("Damage dealt when the Snatcher hits a target during a jump.")]
    public int jumpDamage;

    [Tooltip("Movement speed while chasing the target on the ground.")]
    public float moveSpeed;

    [Tooltip("Cooldown time between consecutive melee attacks.")]
    public float attackCooldown = 1f;

    [Tooltip("Time duration used to calculate jump trajectory toward the target.")]
    public float jumpTime = 0.8f;

    [Header("Snatcher: Landing")]
    [Tooltip("Layer mask used to identify valid jump targets.")]
    public LayerMask targetMask;

    [Tooltip("Force applied when Snatcher rebounds after landing.")]
    public float landJumpForce = 10f;

    [Tooltip("Angle (in degrees) of rebound direction after landing.")]
    public float landJumpAngle = 60f;

    public SnatcherOnWallState onWallState;
    public SnatcherJumpOnTargetState jumpTargetState;
    public SnatcherChaseState chaseState;
    public SnatcherPatrolState patrolState;
    public SnatcherAttackState attackState; 

    public event Action<Collider2D> OnTriggerEntered;
    public event Action<Collider2D> OnTriggerExited;

    protected override void Awake()
    {
        base.Awake();

        onWallState = new SnatcherOnWallState(this);
        jumpTargetState = new SnatcherJumpOnTargetState(this);
        chaseState = new SnatcherChaseState(this);
        patrolState = new SnatcherPatrolState(this);
        attackState = new SnatcherAttackState(this);
    }

    protected override void Start()
    {
        base.Start();
        logicStateMachine.Initialize(onWallState);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggerEntered?.Invoke(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        OnTriggerExited?.Invoke(collision);
    }
}
