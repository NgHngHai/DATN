using UnityEngine;

/// <summary>
/// Ground-based entity that patrols and dashes toward targets if detected.
/// If the target is close enough, it will explode (dash state only).
/// If it loses sight of target, it will go back to patrol.
/// </summary>
public class EnemyBoom : GroundEnemy
{
    [Header("Enemy: Boom")]
    public float chaseSpeed;
    public float explodeDistance;

    public BoomPatrolState patrolState;
    public BoomChaseState chaseState;
    public BoomExplodeState explodeState;

    public AnimationState animRunState;
    public AnimationState animExplodeState;

    protected override void Awake()
    {
        base.Awake();
        patrolState = new BoomPatrolState(this);
        chaseState = new BoomChaseState(this);
        explodeState = new BoomExplodeState(this);

        animRunState = new AnimationState(this, "run");
        animExplodeState = new AnimationState(this, "explode");
    }

    protected override void Start()
    {
        base.Start();
        logicStateMachine.Initialize(patrolState);
        animStateMachine.Initialize(animRunState);
    }

    public override void OnDeath()
    {
        logicStateMachine.ChangeState(explodeState);
    }

    public void DestroyItself() => Destroy(gameObject);
}
