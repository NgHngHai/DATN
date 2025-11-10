using UnityEngine;

/// <summary>
/// Flying enemy starts in a sleep state. 
/// If it detects a target, it will wake up and chase the target forever. 
/// </summary>
public class EnemyFly0 : Enemy
{
    [Header("Enemy: Fly-0")]
    [Tooltip("The distance at which the enemy stops chasing and starts hovering around the target.")]
    public float hoverAroundDistance;

    [Tooltip("How long the enemy rests before choosing a new hover position or attacking.")]
    public float hoverRestTime;

    [Tooltip("Movement speed while hovering around the target.")]
    public float hoverSpeed;

    [Tooltip("Movement speed while chasing the target.")]
    public float chaseSpeed;

    public Fly0SleepState sleepState;
    public Fly0ChaseState chaseState;
    public Fly0HoverAroundState hoverAroundState;
    public Fly0AttackState attackState;

    protected override void Awake()
    {
        base.Awake();
        sleepState = new Fly0SleepState(this);
        chaseState = new Fly0ChaseState(this);
        hoverAroundState = new Fly0HoverAroundState(this);
        attackState = new Fly0AttackState(this);
    }

    protected override void Start()
    {
        base.Start();
        logicStateMachine.Initialize(sleepState);
    }
}
