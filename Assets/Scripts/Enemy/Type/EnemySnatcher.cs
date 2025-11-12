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

    [Tooltip("Time duration used to calculate jump trajectory toward the target.")]
    public float jumpTime = 0.8f;

    [Header("Snatcher: Landing")]
    [Tooltip("Force applied when Snatcher rebounds after landing.")]
    public float reboundForce = 10f;

    [Tooltip("Angle (in degrees) of rebound direction after landing.")]
    public float reboundAngle = 60f;

    [Tooltip("Secondary collider helps snatcher determine when to rebound")]
    public SnatcherSecondaryCollider secondaryCollider;

    public SnatcherOnWallState onWallState;
    public SnatcherJumpOnTargetState jumpTargetState;
    public SnatcherReboundState reboundState;
    public SnatcherChaseState chaseState;
    public SnatcherPatrolState patrolState;
    public SnatcherAttackState attackState;

    protected override void Awake()
    {
        base.Awake();

        onWallState = new SnatcherOnWallState(this);
        jumpTargetState = new SnatcherJumpOnTargetState(this);
        reboundState = new SnatcherReboundState(this);
        chaseState = new SnatcherChaseState(this);
        patrolState = new SnatcherPatrolState(this);
        attackState = new SnatcherAttackState(this);
    }

    protected override void Start()
    {
        base.Start();
        logicStateMachine.Initialize(onWallState);
    }
}
