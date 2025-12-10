using UnityEngine;

public class BossBowstring : Enemy
{
    [Header("Boss: Bowstring")]
    [Range(0f, 1f)]
    [SerializeField] private float aggressiveRate;
    public Vector2 maxThinkTimeRange = new Vector2(2f, 3f);
    public Vector2 minThinkTimeRange = new Vector2(0.5f, 1f);
    public Vector2 doubleSlashOffset = new Vector2(-3f, -3f);
    public Vector2 pokeOffset = new Vector2(-1f, 1.5f);
    public float chaseSpeed = 7f;
    public float approachSpeed = 3f;
    public float approachHeightOffset = 1f;
    public float makeAttackDecisionDistance = 5;

    public bool canBurrowAttack { get; set; }
    public Vector2 thinkTimeRange { get; set; }

    public AnimationState animAppearState;
    public AnimationState animIdleState;
    public AnimationState animMoveState;
    public AnimationState animPokeAttackState;
    public AnimationState animDoubleSlashAttackState;
    public AnimationState animBurrowAttackState;

    public BowstringAppearState appearState;
    public BowstringThinkState thinkState;
    public BowstringApproachState approachState;
    public BowstringChaseState chaseState;
    public BowstringAttackState pokeAttackState;
    public BowstringAttackState doubleSlashAttackState;
    public BowstringAttackState burrowAttackState;


    protected override void Awake()
    {
        base.Awake();

        thinkTimeRange = maxThinkTimeRange;

        animAppearState = new AnimationState(this, "appear");
        animIdleState = new AnimationState(this, "idle");
        animMoveState = new AnimationState(this, "move");
        animPokeAttackState = new AnimationState(this, "attackPoke");
        animDoubleSlashAttackState = new AnimationState(this, "attackDoubleSlash");
        animBurrowAttackState = new AnimationState(this, "attackBurrow");

        appearState = new BowstringAppearState(this);
        thinkState = new BowstringThinkState(this);
        approachState = new BowstringApproachState(this);
        chaseState = new BowstringChaseState(this);
        pokeAttackState = new BowstringAttackState(this, animPokeAttackState, 0);
        doubleSlashAttackState = new BowstringAttackState(this, animDoubleSlashAttackState, 1);
        burrowAttackState = new BowstringAttackState(this, animBurrowAttackState, 2);
    }

    protected override void Start()
    {
        base.Start();
        animStateMachine.Initialize(animAppearState);
        logicStateMachine.Initialize(appearState);
    }

    public void UnlockBurrowAttack()
    {
        canBurrowAttack = true;
    }

    public float AggresiveRate
    {
        get { return aggressiveRate; }
        set { 
            aggressiveRate = value;

            if (aggressiveRate < 0.5f)
                thinkTimeRange = maxThinkTimeRange;
            else
                thinkTimeRange = Vector2.Lerp(maxThinkTimeRange, minThinkTimeRange, aggressiveRate);
        }
    }
}
