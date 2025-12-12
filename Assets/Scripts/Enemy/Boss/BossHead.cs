using UnityEngine;
using System.Collections;

public class BossHead : GroundEnemy
{
    [Header("Boss: Head")]
    public float moveSpeed = 7f;
    public GameObject appearEffect;
    public ParticleSystem runningDustPS;
    public ParticleSystem blowingUpSmokePS;

    public AnimationState animInactiveState;
    public AnimationState animReviveState;
    public AnimationState animMoveState;
    public AnimationState animAttackState;

    public HeadInactiveState inactiveState;
    public HeadReviveState reviveState;
    public HeadMoveState moveState;
    public HeadAttackState attackState;

    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(WaitThenDisableBlowingSmoke());

        animInactiveState = new AnimationState(this, "inactive");
        animReviveState = new AnimationState(this, "revive");
        animMoveState = new AnimationState(this, "move");
        animAttackState = new AnimationState(this, "attack");

        inactiveState = new HeadInactiveState(this);
        reviveState = new HeadReviveState(this);
        moveState = new HeadMoveState(this);
        attackState = new HeadAttackState(this);
    }

    protected override void Start()
    {
        base.Start();

        animStateMachine.Initialize(animInactiveState);
        logicStateMachine.Initialize(inactiveState);

        StartAppearEffect();
    }

    private void StartAppearEffect()
    {
        Instantiate(appearEffect, transform.position, Quaternion.identity);

        rb.angularVelocity = Random.Range(-400f, 400f);

        float angle = Random.Range(45f, 125f);
        float rad = angle * Mathf.Deg2Rad;
        Vector2 breakDir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;

        rb.AddForce(breakDir * 15f, ForceMode2D.Impulse);
    }

    IEnumerator WaitThenDisableBlowingSmoke()
    {
        var emission = blowingUpSmokePS.emission;
        yield return new WaitForSeconds(1);
        emission.rateOverTime = 20;
        yield return new WaitForSeconds(1);
        emission.rateOverTime = 10;
        yield return new WaitForSeconds(1);
        emission.rateOverTime = 5;
        yield return new WaitForSeconds(1);
        blowingUpSmokePS.Stop();
    }
}
