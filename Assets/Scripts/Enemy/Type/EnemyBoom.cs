using UnityEngine;

/// <summary>
/// Ground-based enemy that patrols and dashes toward targets if detected.
/// If the target is close enough, it will explode (dash state only).
/// If it loses sight of target, it will go back to patrol.
/// </summary>
public class EnemyBoom : GroundEnemy
{
    [Header("Enemy: Boom")]
    [Tooltip("Maximum dash speed reached during the dash.")]
    public float maxDashSpeed;

    [Tooltip("How long the dash lasts before stopping or switching state.")]
    public float dashDuration;

    [Tooltip("Curve controlling dash speed progression over time.")]
    public AnimationCurve dashCurve;

    [Tooltip("Distance from target at which the enemy explodes.")]
    public float explodeDistance;

    [Tooltip("Prefab used to create the explosion effect on death or self-destruct.")]
    [SerializeField] private GameObject explosionPrefab;

    public BoomPatrolState patrolState;
    public BoomDashState dashState;
    public BoomTargetDetectedState targetDetectedState;

    protected override void Awake()
    {
        base.Awake();
        patrolState = new BoomPatrolState(this);
        dashState = new BoomDashState(this);
        targetDetectedState = new BoomTargetDetectedState(this);
    }

    protected override void Start()
    {
        base.Start();
        logicStateMachine.Initialize(patrolState);
    }

    public override void OnDeath()
    {
        logicStateMachine.currentState?.Exit();
        StopVelocity();
        Invoke(nameof(Explode), 0.5f);
    }

    public void Explode()
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
