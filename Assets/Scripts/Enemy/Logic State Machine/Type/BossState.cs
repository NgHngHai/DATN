using System.Collections.Generic;
using UnityEngine;

public class BossState : EnemyOffensiveState
{
    protected Boss boss;
    protected int handAttackIndex = 0;
    protected int dashAttackIndex = 1;
    protected int nukeAttackIndex = 2;

    public BossState(Enemy enemy) : base(enemy)
    {
        boss = enemy as Boss;
    }
}

public class BossRestState : BossState
{
    public BossRestState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        boss.StopVelocity();
        animStateMachine.ChangeState(boss.animIdleState);
    }
}

public class BossMoveState : BossState
{
    private AnimationCurve moveCurve;

    private Vector2 startPos;
    private Vector2 targetPos;

    private float moveDuration;

    public BossMoveState(Enemy enemy) : base(enemy)
    {
        moveCurve = boss.moveCurve;
        moveDuration = boss.moveDuration;
    }

    public override void Enter()
    {
        base.Enter();

        startPos = boss.transform.position;
        stateTimer = moveDuration;
    }

    public override void Update()
    {
        base.Update();

        float t = (moveDuration - stateTimer) / moveDuration;
        Vector2 movePos = Vector2.Lerp(startPos, targetPos, moveCurve.Evaluate(t));

        boss.rb.MovePosition(movePos);

        if (stateTimer < 0)
        {
            logicStateMachine.ChangeState(boss.restState);
        }
    }

    public void SetTargetPos(Vector2 targetPos) => this.targetPos = targetPos;
}

public class BossHandAttackState : BossState
{
    public BossHandAttackState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        if (boss.IsPhaseTwoOrAbove())
        {
            logicStateMachine.ChangeState(boss.restState);
            return;
        }

        animStateMachine.ChangeState(boss.animHandAttack);

        attackSet.ChangeAttackType(handAttackIndex);
    }

    public override void Update()
    {
        base.Update();

        if (boss.IsCurrentAnimStateTriggerCalled())
        {
            logicStateMachine.ChangeState(boss.restState);
        }
    }
}

public class BossDashAttackState : BossState
{
    public BossDashAttackState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        if (boss.IsPhaseTwoOrAbove())
        {
            animStateMachine.ChangeState(boss.animBodyDash);
        }
        else
        {
            animStateMachine.ChangeState(boss.animHandDash);
        }

        attackSet.ChangeAttackType(dashAttackIndex);
    }

    public override void Update()
    {
        base.Update();

        if (boss.IsCurrentAnimStateTriggerCalled())
        {
            logicStateMachine.ChangeState(boss.restState);
        }
    }
}

public class BossNukeAttackState : BossState
{
    private List<GameObject> currentSafePlatformObjects = new List<GameObject>();

    public BossNukeAttackState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        if (boss.IsPhaseTwoOrAbove())
        {
            animStateMachine.ChangeState(boss.animBodyNuke);
        }
        else
        {
            animStateMachine.ChangeState(boss.animHandNuke);
        }
        attackSet.ChangeAttackType(nukeAttackIndex);
        ToggleSafePlatforms(true);
    }

    public override void Update()
    {
        base.Update();

        if (boss.IsCurrentAnimStateTriggerCalled())
        {
            logicStateMachine.ChangeState(boss.restState);
        }
    }

    public override void Exit()
    {
        base.Exit();

        ToggleSafePlatforms(false);
    }

    private void ToggleSafePlatforms(bool toggle)
    {
        if (currentSafePlatformObjects.Count == 0)
        {
            currentSafePlatformObjects = boss.GetSpawnSafePlatform();
        }

        foreach (GameObject platform in currentSafePlatformObjects)
        {
            platform.SetActive(toggle);
        }
    }
}