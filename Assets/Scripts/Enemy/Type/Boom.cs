using UnityEngine;

/// <summary>
/// Ground-based enemy that patrols and dashes toward targets if detected.
/// If the target is close enough, it will explode (dash state only)
/// If it loses sight of target, it will go back to patrol.
/// </summary>
public class Boom : GroundEnemy
{
    [Header("Boom")]
    public float maxDashSpeed;
    public float dashDuration;
    public AnimationCurve dashCurve;
    public float explodeDistance;
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

    public void Explode()
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
