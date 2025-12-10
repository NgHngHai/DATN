using Unity.VisualScripting;
using UnityEngine;

public abstract class BossBehavior
{
    protected EnemyTargetHandler targetHandler;
    protected Boss boss;

    private float cooldown = 0f;
    private float nextAvailableTime = 0f;

    public BossBehavior(Boss boss, float cooldown)
    {
        this.boss = boss;
        targetHandler = boss.TargetHandler;
        this.cooldown = cooldown;
    }

    public float BaseEvaluate()
    {
        if (Time.time < nextAvailableTime || !targetHandler.IsTargetValid()) return 0f;
        return Evaluate();
    }

    public void BaseExecute()
    {
        nextAvailableTime = Time.time + cooldown;
        Execute();
    }

    protected abstract float Evaluate();
    protected abstract void Execute();
}

public class BossHandAttackBehavior : BossBehavior
{
    private float preferredAttackRadius;

    /// <param name="preferredAttackRadius">Score higher when the distance between target and boss is SMALLER than this distance</param>
    public BossHandAttackBehavior(Boss boss, float cooldown, float preferredAttackRadius) : base(boss, cooldown)
    {
        this.preferredAttackRadius = preferredAttackRadius;
    }

    protected override float Evaluate()
    {
        if (boss.CurrentPhase != 1 || !targetHandler.IsTargetValid()) return 0f;

        if (Vector2.Distance(targetHandler.GetTargetPosition(),
            boss.frontHandImpactPoint.position) < preferredAttackRadius) return 80f;
        else return 0f;
    }

    protected override void Execute()
    {
        boss.logicStateMachine.ChangeState(boss.handAttackState);
    }
}

public class BossDashAttackBehavior : BossBehavior
{
    private float preferredDashDistance;

    /// <param name="preferredDashDistance">Score higher when the distance between target and boss is GREATER than this distance</param>
    public BossDashAttackBehavior(Boss boss, float cooldown, float preferredDashDistance) : base(boss, cooldown)
    {
        this.preferredDashDistance = preferredDashDistance;
    }

    protected override float Evaluate()
    {
        if (targetHandler.GetHorizontalDirectionToTarget() == 1) return 0f;

        if (targetHandler.GetDistanceToTarget() > preferredDashDistance) return 70f;

        return 30f;
    }

    protected override void Execute()
    {
        if (!targetHandler.IsTargetValid()) return;

        boss.logicStateMachine.ChangeState(boss.dashAttackState);
    }
}

public class BossMoveBackBehavior : BossBehavior
{
    private float moveDistance;

    public BossMoveBackBehavior(Boss boss, float cooldown, float moveDistance) : base(boss, cooldown)
    {
        this.moveDistance = moveDistance;
    }

    protected override float Evaluate()
    {
        float dist = targetHandler.GetDistanceToTarget();
        bool isTargetBehindBoss = targetHandler.GetHorizontalDirectionToTarget() > 0;

        if (!isTargetBehindBoss && dist > 3f) return 0f;

        if (dist > 6f) return 30f;
        else if (dist > 3f && dist <= 6f) return 50f;

        return 70f;
    }

    protected override void Execute()
    {
        Vector2 moveBackPos = boss.transform.position;
        moveBackPos.x += moveDistance;

        boss.moveState.SetTargetPos(moveBackPos);
        boss.logicStateMachine.ChangeState(boss.moveState);
    }
}

public class BossNukeAttackBehavior : BossBehavior
{
    public BossNukeAttackBehavior(Boss boss, float cooldown) : base(boss, cooldown)
    {
    }

    protected override float Evaluate()
    {
        return Random.Range(20f, 30f);
    }

    protected override void Execute()
    {
        boss.logicStateMachine.ChangeState(boss.nukeAttackState);
    }
}

public class BossAdjustBowstringAggressiveBehavior : BossBehavior
{
    private float closeDist;
    private float farDist;
    private bool stopAggressive;

    public BossAdjustBowstringAggressiveBehavior(Boss boss, float cooldown, float closeDist, float farDist)
        : base(boss, cooldown)
    {
        this.closeDist = closeDist;
        this.farDist = farDist;
    }

    protected override float Evaluate()
    {
        if (boss.Bowstring == null) return 0f;

        if (boss.logicStateMachine.currentState == boss.nukeAttackState)
        {
            stopAggressive = true;
            return 100f;
        }

        return Random.Range(30f, 70f);
    }

    protected override void Execute()
    {
        if (stopAggressive)
        {
            SetAggressiveRate(0);
            stopAggressive = false;
        }

        float dist = targetHandler.GetDistanceToTarget();

        if (dist < closeDist)
            SetAggressiveRate(1f);
        else if (dist > farDist)
            SetAggressiveRate(0.5f);
        else 
            SetAggressiveRate(0.8f);
    }

    private void SetAggressiveRate(float value)
    {
        boss.Bowstring.AggresiveRate = value;
    }
}
