using UnityEngine;

public class EnemyFly0 : Enemy
{
    [Header("Enemy: Fly-0")]
    [Tooltip("The distance at which the enemy stops chasing and starts hovering around the target.")]
    public float hoverAroundDistance;

    [Tooltip("How long the enemy rests before choosing a new hover position or attacking.")]
    public float hoverRestTime;

    public float hoverSpeed;
    public float chaseSpeed;

    public Fly0SleepState sleepState;
    public Fly0ChaseState chaseState;
    public Fly0HoverAroundState hoverAroundState;
    public Fly0AttackState attackState;

    public AnimationState animAwakeState;
    public AnimationState animSleepingState;
    public AnimationState animFlyingState;
    public AnimationState animAttackState;

    protected override void Awake()
    {
        base.Awake();
        sleepState = new Fly0SleepState(this);
        chaseState = new Fly0ChaseState(this);
        hoverAroundState = new Fly0HoverAroundState(this);
        attackState = new Fly0AttackState(this);

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
