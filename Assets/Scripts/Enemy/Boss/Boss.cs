using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BossBrain))]
public class Boss : Enemy
{
    [Header("BOSS")]
    [SerializeField] private GameObject bowstringPrefab;
    [SerializeField] private Transform bowstringSpawnPos;
    [SerializeField] private List<BossHandBreakAnimation> handBreakAnimationList;

    [Header("BOSS: Head Properties")]
    [SerializeField] private SpriteRenderer headSpriteRenderer;
    [SerializeField] private Sprite brokenHeadSprite;
    [SerializeField] private GameObject headPrefab;
    [SerializeField] private GameObject headBrokenEffect;
    public Transform frontHandImpactPoint;

    [Header("BOSS: Movement Curve")]
    public float moveDuration = 3f;
    public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("BOSS: Safe Platform")]
    [SerializeField] private GameObject safePlatformPrefab;
    [SerializeField] private Transform[] safePlatformSpawnPointArray;

    public AnimationState animIdleState;
    public AnimationState animHandAttackState;
    public AnimationState animHandDashState;
    public AnimationState animHandNukeState;
    public AnimationState animBodyDashState;
    public AnimationState animBodyNukeState;
    public AnimationState animDeathState;

    public BossRestState restState;
    public BossMoveState moveState;
    public BossHandAttackState handAttackState;
    public BossDashAttackState dashAttackState;
    public BossNukeAttackState nukeAttackState;

    private BossBrain brain;
    private BossBowstring bowstring;
    private BossHead head;

    private int currentPhase = 1;

    protected override void Awake()
    {
        base.Awake();

        brain = GetComponent<BossBrain>();

        animIdleState = new AnimationState(this, "idle");
        animHandAttackState = new AnimationState(this, "handAttack");
        animHandDashState = new AnimationState(this, "handDash");
        animHandNukeState = new AnimationState(this, "handNuke");
        animBodyDashState = new AnimationState(this, "bodyDash");
        animBodyNukeState = new AnimationState(this, "bodyNuke");
        animDeathState = new AnimationState(this, "death");

        restState = new BossRestState(this);
        moveState = new BossMoveState(this);
        handAttackState = new BossHandAttackState(this);
        dashAttackState = new BossDashAttackState(this);
        nukeAttackState = new BossNukeAttackState(this);
    }

    protected override void Start()
    {
        base.Start();

        animStateMachine.Initialize(animIdleState);
        logicStateMachine.Initialize(restState);

        brain.Initialize(this);
    }


    protected override void OnDamageTaken(int appliedAmount, Vector2 hitDir, bool shouldTriggerHitReaction)
    {
        base.OnDamageTaken(appliedAmount, hitDir, shouldTriggerHitReaction);

        if (currentPhase == 3) return;

        float hp = (float)health.currentHealth / health.maxHealth;

        if (currentPhase == 1 && hp < 2f / 3f)
            ToPhaseTwo();
        // If first damage amount is 2/3 of health percent, this if will not be called
        else if (currentPhase == 2 && hp < 1f / 3f)
            ToPhaseThree();
    }

    public void ToPhaseTwo()
    {
        currentPhase = 2;

        headSpriteRenderer.sprite = brokenHeadSprite;
        Instantiate(headBrokenEffect, headSpriteRenderer.transform.position, Quaternion.identity);

        foreach (var handBreakAnimation in handBreakAnimationList)
        {
            handBreakAnimation.StartAnimation();
        }

        GameObject bowstringObject = Instantiate(bowstringPrefab, bowstringSpawnPos.position, Quaternion.identity);
        bowstring = bowstringObject.GetComponent<BossBowstring>();
        brain.AcknowledgePhaseTwo();
    }

    public void ToPhaseThree()
    {
        if (currentPhase != 2)
        {
            ToPhaseTwo();
        }

        currentPhase = 3;

        Transform headTransform = headSpriteRenderer.transform;
        Destroy(headTransform.gameObject);

        GameObject headObject = Instantiate(headPrefab, headTransform.position, Quaternion.identity);
        head = headObject.GetComponent<BossHead>();
        bowstring.UnlockBurrowAttack();
    }

    public BossBowstring Bowstring => bowstring;
    public BossHead Head => head;

    public List<GameObject> GetSpawnSafePlatform()
    {
        List<GameObject> res = new List<GameObject>();

        foreach(Transform spawnPoint in safePlatformSpawnPointArray)
        {
            GameObject newPlatform = Instantiate(safePlatformPrefab, spawnPoint.position, Quaternion.identity);
            res.Add(newPlatform);
        }

        return res;
    }

    public override void OnDeath()
    {
        GetComponent<Collider2D>().enabled = false;
        brain.enabled = false;
        animStateMachine.ChangeState(animDeathState);
    }

    public void CreateDeathExplosionInRadius()
    {
        Vector2 randomPos = (Vector2) transform.position+ Random.insideUnitCircle * 5;
        CreateDeathExplosionAt(randomPos);
    }

    public bool IsPhaseTwoOrAbove() => currentPhase == 2 || currentPhase == 3;
    public int CurrentPhase => currentPhase;
}
