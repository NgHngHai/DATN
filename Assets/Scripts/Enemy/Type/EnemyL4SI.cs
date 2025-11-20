using UnityEngine;

public class EnemyL4SI : Enemy
{
    [Header("Enemy: L4-SI")]
    public EnemyIncreaseAttackSpeedAura increaseAttackSpeedAura;
    public int throwWingCount = 3;
    public float attackModeCooldown = 20;
    public float teleportDelay = 0.5f;

    [Header("Burrow Attack")]
    public HurtBox burrowHurtBox;
    public LayerMask burrowMask;
    public float burrowAttackDuration = 1f;
    public AnimationCurve burrowCurve;

    public L4SISupportState supportState;
    public L4SIWingAttackState wingAttackState;
    public L4SIBurrowAttackState burrowAttackState;
    public L4SITeleportAwayState teleportAwayState;


    protected override void Awake()
    {
        base.Awake();
        supportState = new L4SISupportState(this);
        wingAttackState = new L4SIWingAttackState(this);
        burrowAttackState = new L4SIBurrowAttackState(this);
        teleportAwayState = new L4SITeleportAwayState(this);
    }

    protected override void Start()
    {
        base.Start();

        logicStateMachine.Initialize(supportState);
    }

    public void TeleportToClosetEnemy()
    {
        float minDist = float.MaxValue;
        Transform nearest = null;

        var all = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        foreach (var other in all)
        {
            if (other == null || other == this) continue;

            float d = Vector2.Distance(transform.position, other.transform.position);
            if (d < minDist)
            {
                minDist = d;
                nearest = other.transform;
            }
        }

        if(nearest != null) 
            transform.position = nearest.position;
    }
}