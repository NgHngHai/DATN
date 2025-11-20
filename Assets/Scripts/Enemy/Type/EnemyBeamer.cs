using UnityEngine;

/// <summary>
/// A stationary enemy in the air. It has 2 states: Observation and Attack.
/// The enemy is invincible during the Observation state.
/// After attacking, it enters a rest period before observing again.
/// </summary>
public class EnemyBeamer : Enemy
{
    [Header("Enemy: Beamer")]
    [Tooltip("Time between laser shots")]
    public float startAttackDistance = 4f;

    [Tooltip("Rest period after the attack.")]
    public float restTime = 5;

    public AnimationState animClosedState;
    public AnimationState animAttackState;
    public AnimationState animRestState;


    public BeamerObservationState observationState;
    public BeamerAttackState attackState;
    public BeamerRestState restState;

    protected override void Awake()
    {
        base.Awake();

        observationState = new BeamerObservationState(this);
        attackState = new BeamerAttackState(this);
        restState = new BeamerRestState(this);

        animClosedState = new AnimationState(this, "closedMode");
        animAttackState = new AnimationState(this, "attackMode");
        animRestState = new AnimationState(this, "restMode");
    }

    protected override void Start()
    {
        base.Start();
        logicStateMachine.Initialize(observationState);
        animStateMachine.Initialize(animClosedState);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, startAttackDistance);
    }
}
