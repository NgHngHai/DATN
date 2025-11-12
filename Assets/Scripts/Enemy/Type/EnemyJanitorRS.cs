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
    public float teleportCooldown = 3f;

    [Tooltip("Delay time before the teleport action is executed.")]
    public float teleportDelay = 0.3f;

    [Tooltip("Vertical offset applied to the teleport position to adjust height.")]
    public float teleportOffsetY;

    [Tooltip("Transform reference for teleport base position A.")]
    public Transform teleportBaseA;

    [Tooltip("Transform reference for teleport base position B.")]
    public Transform teleportBaseB;

    public JanitorRSIdleState idleState;
    public JanitorRSTeleportState teleportState;

    protected override void Awake()
    {
        base.Awake();
        idleState = new JanitorRSIdleState(this);
        teleportState = new JanitorRSTeleportState(this);
    }

    protected override void Start()
    {
        base.Start();
        logicStateMachine.Initialize(idleState);
    }
}
