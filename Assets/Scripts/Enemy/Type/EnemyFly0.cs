using UnityEngine;

public class EnemyFly0 : Enemy
{
    [Header("Enemy: Fly-0")]
    public float offsetFlyHeight = 1f;
    public float stopChaseDistance = 3f;
    public float attackRestTime = 1.5f;
    public float moveSpeed = 10f;

    public Fly0RepositionToAttackState repositionToAttackState;
    public Fly0SleepState sleepState;
    public Fly0ChaseState chaseState;
    public Fly0AttackState attackState;
    public Fly0RestState restState;

    public AnimationState animAwakeState;
    public AnimationState animSleepingState;
    public AnimationState animFlyingState;
    public AnimationState animAttackState;

    protected override void Awake()
    {
        base.Awake();
        sleepState = new Fly0SleepState(this);
        repositionToAttackState = new Fly0RepositionToAttackState(this);
        chaseState = new Fly0ChaseState(this);
        attackState = new Fly0AttackState(this);
        restState = new Fly0RestState(this);

        animAwakeState = new AnimationState(this, "awake");
        animSleepingState = new AnimationState(this, "sleeping");
        animFlyingState = new AnimationState(this, "flying");
        animAttackState = new AnimationState(this, "attack");
    }

    protected override void Start()
    {
        base.Start();
        logicStateMachine.Initialize(sleepState);
        animStateMachine.Initialize(animSleepingState);
    }
}
 