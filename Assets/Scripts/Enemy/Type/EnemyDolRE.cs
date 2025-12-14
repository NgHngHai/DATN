using UnityEngine;

public class EnemyDolRE : GroundEnemy
{
    [Header("Enemey: Dol-RE")]
    public float prepareAttackTime = 3;

    public AnimationState animIdleState;
    public AnimationState animMoveState;
    public AnimationState animShootExplosiveState;
    public AnimationState animShootHomingState;

    public DolREPatroState patrolState;
    public DolREShootExplosiveState shootExplosiveState;
    public DolREShootHomingMissileState shootHomingState;
    public DolREPrepareToAttackState prepareToAttackState;

    protected override void Awake()
    {
        base.Awake();

        animIdleState = new AnimationState(this, "idle");
        animMoveState = new AnimationState(this, "move");
        animShootExplosiveState = new AnimationState(this, "shootExplosive");
        animShootHomingState = new AnimationState(this, "shootHoming");

        patrolState = new DolREPatroState(this);
        shootExplosiveState = new DolREShootExplosiveState(this);
        shootHomingState = new DolREShootHomingMissileState(this);
        prepareToAttackState = new DolREPrepareToAttackState(this);
    }

    protected override void Start()
    {
        base.Start();
        animStateMachine.Initialize(animIdleState);
        logicStateMachine.Initialize(patrolState);
    }
}