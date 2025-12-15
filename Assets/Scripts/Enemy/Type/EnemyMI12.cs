using UnityEngine;

public class EnemyMI12 : GroundEnemy
{
    [Header("Enemy: MI-12")]
    public AudioClip chargeSound;
    public float minChargeDistance = 3;
    public AnimationCurve chargeSpeedCurve;
    public float chargeDuration = 1f;
    public float maxChargeSpeed = 10f;

    public AnimationState animIdleState;
    public AnimationState animLaserBlastState;
    public AnimationState animChargeState;

    public MI12ObservationState observationState;
    public MI12ChargeState chargeState;
    public MI12BlastLaserAttack blastLaserAttackState;

    protected override void Awake()
    {
        base.Awake();

        animIdleState = new AnimationState(this, "idle");
        animLaserBlastState = new AnimationState(this, "laserBlast");
        animChargeState = new AnimationState(this, "charge");

        observationState = new MI12ObservationState(this);
        chargeState = new MI12ChargeState(this);
        blastLaserAttackState = new MI12BlastLaserAttack(this);
    }

    protected override void Start()
    {
        base.Start();

        animStateMachine.Initialize(animIdleState);
        logicStateMachine.Initialize(observationState);
    }
}

