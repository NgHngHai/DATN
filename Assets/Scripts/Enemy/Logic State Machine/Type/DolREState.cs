using Unity.VisualScripting;
using UnityEngine;

public class DolREState : EnemyOffensiveState
{
    protected EnemyDolRE dolre;

    public DolREState(Enemy enemy) : base(enemy)
    {
        dolre = enemy as EnemyDolRE;
    }
}

public class DolREPatroState : DolREState
{
    private EnemyPatrolBehavior patrolBehavior;

    public DolREPatroState(Enemy enemy) : base(enemy)
    {
        patrolBehavior = dolre.GetComponent<EnemyPatrolBehavior>();
    }

    public override void Enter()
    {
        base.Enter();
        patrolBehavior.StartPatrolProcess();
    }

    public override void Update()
    {
        base.Update();

        if (dolre.IsIdle())
            animStateMachine.ChangeState(dolre.animIdleState);
        else animStateMachine.ChangeState(dolre.animMoveState);

        if (IsTargetValid())
            logicStateMachine.ChangeState(dolre.prepareToAttackState);
    }

    public override void Exit()
    {
        base.Exit();
        patrolBehavior.StopPatrolProcess();
        dolre.StopVelocity();
    }
}

public class DolREPrepareToAttackState : DolREState
{
    private bool isHomingNext;

    public DolREPrepareToAttackState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        animStateMachine.ChangeState(dolre.animIdleState);
        stateTimer = dolre.prepareAttackTime;
    }

    public override void Update()
    {
        base.Update();

        if (!IsTargetValid())
        {
            logicStateMachine.ChangeState(dolre.patrolState);
            return;
        }

        if (stateTimer < 0)
        {
            FlipToTarget();
            EnemyState nextState = isHomingNext ? dolre.shootHomingState : dolre.shootExplosiveState;
            isHomingNext = !isHomingNext;
            logicStateMachine.ChangeState(nextState);
        }
    }
}

public class DolREShootExplosiveState : DolREState
{
    protected int explosiveIndex = 0;
    private Vector3 baseDirection;
    private bool lookRight;
    private float halfAngle = 60f;

    public DolREShootExplosiveState(Enemy enemy) : base(enemy)
    {
        baseDirection = Vector2.right;
    }

    public override void Enter()
    {
        base.Enter();

        animStateMachine.ChangeState(dolre.animShootExplosiveState);
        attackSet.ChangeAttackType(explosiveIndex);

        Vector3 randomDir = Quaternion.Euler(0f, 0f, 60) * baseDirection;

        attackSet.CurrentAttack.AttackPointLookAtDirection(randomDir);
    }

    public override void Update()
    {
        base.Update();

        //AttackPointLookAtRandomAngle();

        if (dolre.IsCurrentAnimStateTriggerCalled())
        {
            logicStateMachine.ChangeState(dolre.prepareToAttackState);
        }
    }

    private void AttackPointLookAtRandomAngle()
    {
        baseDirection.x = lookRight ? 1 : -1;
        lookRight = !lookRight;

        float angle = Random.Range(-halfAngle, halfAngle);

        Vector3 randomDir = Quaternion.Euler(0f, 0f, angle) * baseDirection;

        attackSet.CurrentAttack.AttackPointLookAtDirection(randomDir);
    }
}

public class DolREShootHomingMissileState : DolREState
{
    private int homingIndex = 1;
    private Vector3 baseDirection;
    private float randomAngle = 60f;

    public DolREShootHomingMissileState(Enemy enemy) : base(enemy)
    {
        baseDirection = Vector2.one;
    }

    public override void Enter()
    {
        base.Enter();

        baseDirection.x = dolre.FacingDir;
        animStateMachine.ChangeState(dolre.animShootHomingState);
        attackSet.ChangeAttackType(homingIndex);
    }

    public override void Update()
    {
        base.Update();

        AttackPointLookAtRandomAngle();

        if (dolre.IsCurrentAnimStateTriggerCalled())
        {
            logicStateMachine.ChangeState(dolre.prepareToAttackState);
        }
    }

    private void AttackPointLookAtRandomAngle()
    {
        float angle = Random.Range(-randomAngle, randomAngle);

        Vector3 randomDir = Quaternion.Euler(0f, 0f, angle) * baseDirection;

        attackSet.CurrentAttack.AttackPointLookAtDirection(randomDir);
    }
}

