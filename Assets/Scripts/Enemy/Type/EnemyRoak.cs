using UnityEngine;

/// <summary>
/// Ground enemy that only patrols and releases poisonous smoke after a delay.
/// Poison smoke deals damage over time.
/// </summary>
public class EnemyRoak : GroundEnemy
{
    [Header("Enemy: Roak")]

    public RoakState mainState;

    public AnimationState animIdleState;
    public AnimationState animRunState;

    protected override void Awake()
    {
        base.Awake();
        mainState = new RoakState(this);

        animIdleState = new AnimationState(this, "idle");
        animRunState = new AnimationState(this, "run");
    }

    protected override void Start()
    {
        base.Start();
        logicStateMachine.Initialize(mainState);
        animStateMachine.Initialize(animIdleState);
    }
}
