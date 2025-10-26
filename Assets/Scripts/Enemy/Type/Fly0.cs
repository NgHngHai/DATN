using UnityEngine;

/// <summary>
/// Flying enemy starts in a sleep state. 
/// If it detects a target, it will wake up and chase the target forever. 
/// </summary>
public class Fly0 : Enemy
{
    [Header("Fly-0")]
    public float hoverAroundDistance;
    public float hoverRestTime;
    public float chaseSpeed;
    public float attackCooldown;

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
