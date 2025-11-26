using UnityEngine;

public class EnemySnatcher : GroundEnemy
{
    [Header("Enemy: Snatcher")]
    public float moveSpeed;
    public float jumpTime = 0.8f;

    [Header("Snatcher: Landing")]
    public float reboundForce = 10f;
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
