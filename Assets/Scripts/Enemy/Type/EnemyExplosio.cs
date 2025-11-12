using UnityEngine;

/// <summary>
/// A flying enemy that alternates between two attacks based on target distance:
/// Stomp (far) and Brust Fire (close).
/// After each attack, it rests briefly before observing again.
/// </summary>
public class EnemyExplosio : Enemy
{
    [Header("Enemy: Explosio")]
    [Tooltip("Time the enemy remains idle after completing an attack before acting again.")]
    public float restTime = 1f;

    [Tooltip("Movement speed when flying toward the target.")]
    public float flySpeed = 3f;

    [Header("Attack: Stomp")]
    [Tooltip("This hurt box is only enabled when it's in Stomp State")]
    public HurtBox stompHurtBox;

    [Tooltip("Duration of the stomp attack from start to impact.")]
    public float stompDuration = 1.2f;

    [Tooltip("Maximum height reached during the stomp jump.")]
    public float maxStompHeight = 4f;

    [Tooltip("Vertical motion curve controlling the stomp height over time.")]
    public AnimationCurve stompHeightCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Tooltip("Distance from the target required to trigger the stomp attack.")]
    public float stompDistance = 3f;

    [Tooltip("Damage dealt by the stomp attack upon impact.")]
    public int stompDamage = 10;

    [Tooltip("Effective radius around impact point where stomp damage is applied.")]
    public float stompRadius = 1.5f;

    [Header("Attack: Brust Fire")]
    [Tooltip("Maximum distance at which the enemy can perform the brust fire attack.")]
    public float canBrustFireDistance = 3f;

    [Tooltip("Maximum number of projectiles fired during a single brust fire sequence.")]
    public int maxFireCount = 30;

    [Tooltip("Time delay between consecutive projectile shots in brust fire mode.")]
    public float fireRate = 0.2f;

    [Tooltip("Maximum random spread angle (in degrees) applied to each fired projectile.")]
    public float fireAngle = 30;

    public ExplosioObservationState observationState;
    public ExplosioStompApproachState stompApproachState;
    public ExplosioStompAttackState stompAttackState;
    public ExplosioBrustFireAttackState brustFireAttackState;
    public ExplosioRestState restState;

    protected override void Awake()
    {
        base.Awake();

        observationState = new ExplosioObservationState(this);
        stompApproachState = new ExplosioStompApproachState(this);
        stompAttackState = new ExplosioStompAttackState(this);
        restState = new ExplosioRestState(this);
        brustFireAttackState = new ExplosioBrustFireAttackState(this);
    }

    protected override void Start()
    {
        base.Start();
        stompHurtBox.ToggleHurtCollider(false);
        logicStateMachine.Initialize(observationState);
    }
}