using UnityEngine;

/// <summary>
/// A stationary enemy in the air. It has 2 states: Observation and Attack.
/// The enemy is invincible during the Observation state.
/// After attacking, it enters a rest period before observing again.
/// </summary>
public class EnemyBeamer : Enemy
{
    [Header("Enemy: Beamer")]
    [Tooltip("Number of consecutive laser shots.")]
    public int consecutiveAttackCount = 3;

    [Tooltip("Initial delay before consecutive laser shots.")]
    public float firstConsecutiveAttackDelay = 0.5f;

    [Tooltip("Time between laser shots")]
    public float attackInterval = 0.3f;

    [Tooltip("Time between laser shots")]
    public float startAttackDistance = 4f;

    [Tooltip("Rest period after the attack.")]
    public float restTime = 5;

    public BeamerObservationState observationState;
    public BeamerAttackState attackState;

    protected override void Awake()
    {
        base.Awake();

        observationState = new BeamerObservationState(this);
        attackState = new BeamerAttackState(this);
    }

    protected override void Start()
    {
        base.Start();
        logicStateMachine.Initialize(observationState);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, startAttackDistance);
    }
}
