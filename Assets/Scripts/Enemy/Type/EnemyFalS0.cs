using UnityEngine;

public class EnemyFalS0 : Enemy
{
    [Header("Enemy: Fal-S0")]
    public float moveSpeed;
    public GameObject afterImage;

    public FalS0ChaseState chaseState;
    public FalS0MeleeAttackState meleeAttackState;
    public FalS0BlockState blockState;
    public FalS0CounterAttackState counterAttackState;

    protected override void Awake()
    {
        base.Awake();
        chaseState = new FalS0ChaseState(this);
        meleeAttackState = new FalS0MeleeAttackState(this);
        blockState = new FalS0BlockState(this);
        counterAttackState = new FalS0CounterAttackState(this);
    }

    protected override void Start()
    {
        base.Start();
        logicStateMachine.Initialize(chaseState);
    }
}