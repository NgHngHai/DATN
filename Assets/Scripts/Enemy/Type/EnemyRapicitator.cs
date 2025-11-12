using UnityEngine;

public class EnemyRapicitator : GroundEnemy
{
    [Header("Enemy: Rapicitator")]
    [Tooltip("Movement speed when chasing the target.")]
    public float chaseSpeed = 4f;

    [Tooltip("Movement speed when fleeing after a melee attack.")]
    public float fleeSpeed = 6f;

    [Header("Attack: Laser")]
    [Tooltip("Rest duration after firing a laser before chasing again.")]
    public float shootLaserRestTime = 0.7f;

    [Tooltip("Minimum distance to flee before performing a laser attack.")]
    public float minFleeShootLaserDistance = 3f;

    public RapicitatorPatrolState patrolState;
    public RapicitatorChaseState chaseState;
    public RapicitatorFleeState fleeState;
    public RapicitatorLegAttackState legAttackState;
    public RapicitatorLaserAttackState laserAttackState;

    protected override void Awake()
    {
        base.Awake();

        patrolState = new RapicitatorPatrolState(this);
        chaseState = new RapicitatorChaseState(this);
        fleeState = new RapicitatorFleeState(this);
        legAttackState = new RapicitatorLegAttackState(this);
        laserAttackState = new RapicitatorLaserAttackState(this);
    }

    protected override void Start()
    {
        base.Start();

        logicStateMachine.Initialize(patrolState);
    }
}
