using UnityEngine;

public class EnemyMI12 : GroundEnemy
{
    [Header("Enemy: MI-12")]
    public float decideToChargeDistance = 3;
    public float chargeRestTime = 1f;
    public AnimationCurve chargeSpeedCurve;
    public float reachMaxSpeedTime = 1f;
    public float maxChargeSpeed = 10f;

    public MI12RestState restState;
    public MI12ChargeState chargeState;
    public MI12BlastLaserAttack blastLaserAttackState;

    protected override void Awake()
    {
        base.Awake();

        restState = new MI12RestState(this);
        chargeState = new MI12ChargeState(this);
        blastLaserAttackState = new MI12BlastLaserAttack(this);
    }

    protected override void Start()
    {
        base.Start();
        logicStateMachine.Initialize(restState);
    }
}

