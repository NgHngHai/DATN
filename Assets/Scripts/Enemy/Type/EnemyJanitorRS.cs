using UnityEngine;

/// <summary>
/// Enemy that teleports between two base points when damaged.
/// After teleporting it can not teleport again after a while.
/// Shield Logic is NOT implemented yet.
/// </summary>
public class EnemyJanitorRS : Enemy
{
    [Header("Enemy: Janitor - RS")]
    [Tooltip("Cooldown duration before the enemy can teleport again after being damaged.")]
    public float teleportCooldown = 10f;

    //[Tooltip("Vertical offset applied to the teleport position to adjust height.")]
    //public float teleportOffsetY;

    [Header("Teleport Bases")]
    public Transform teleportBaseA;
    public Transform teleportBaseB;

    public JanitorRSIdleState idleState;
    public JanitorRSTeleportState teleportState;

    public AnimationState animIdleState;
    public AnimationState animRunState;
    public AnimationState animSpawnState;

    protected override void Awake()
    {
        base.Awake();
        idleState = new JanitorRSIdleState(this);
        teleportState = new JanitorRSTeleportState(this);

        animIdleState = new AnimationState(this, "idle");
        animRunState = new AnimationState(this, "run");
        animSpawnState = new AnimationState(this, "spawn");
    }

    protected override void Start()
    {
        base.Start();
        animStateMachine.Initialize(animIdleState);

        // First state must be the Teleport state to ensure:
        // - JanitorRS teleports to first base (base A) and plays the equip anim for that base.
        logicStateMachine.Initialize(teleportState);
    }

}
