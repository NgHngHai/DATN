using UnityEngine;

public class HeadState : EnemyOffensiveState
{
    protected BossHead head;

    public HeadState(Enemy enemy) : base(enemy)
    {
        head = enemy as BossHead;
    }
}

public class HeadInactiveState : HeadState
{
    private float rotateSpeed = 720f;

    public HeadInactiveState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        base.Enter();

        stateTimer = 2f;

        head.rb.freezeRotation = false;
    }

    public override void Update()
    {
        if (!head.IsGrounded())
            return;

        base.Update();

        if (stateTimer < 0)
        {
            float currentZ = head.transform.eulerAngles.z;

            float newZ = Mathf.MoveTowardsAngle(
                currentZ,
                0f,
                rotateSpeed * Time.deltaTime
            );

            head.transform.rotation = Quaternion.Euler(0, 0, newZ);

            if (Mathf.Abs(Mathf.DeltaAngle(newZ, 0f)) < 1f)
            {
                head.transform.rotation = Quaternion.Euler(0, 0, 0);

                logicStateMachine.ChangeState(head.reviveState);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        head.rb.freezeRotation = true;
    }
}


public class HeadReviveState : HeadState
{
    public HeadReviveState(Enemy enemy) : base(enemy)
    {
    }
    public override void Enter()
    {
        base.Enter();

        animStateMachine.ChangeState(head.animReviveState);
    }

    public override void Update()
    {
        base.Update();

        if (head.IsCurrentAnimStateTriggerCalled())
        {
            logicStateMachine.ChangeState(head.moveState);
        }
    }

}

public class HeadMoveState : HeadState
{
    private int moveDir = 1;
    private ParticleSystem.EmissionModule runningDustEmission;

    public HeadMoveState(Enemy enemy) : base(enemy)
    {
        runningDustEmission = head.runningDustPS.emission;
    }

    public override void Enter()
    {
        base.Enter();

        runningDustEmission.rateOverTime = 10f;
        animStateMachine.ChangeState(head.animMoveState);
        moveDir = Random.Range(0, 2) == 0 ? 1 : -1;
    }

    public override void Update()
    {
        base.Update();

        if (head.IsGroundEdgeOrWallDetected())
        {
            head.Flip();
            moveDir *= -1;
        }

        head.SetVelocityX(moveDir * head.moveSpeed);

        if (IsTargetValid() && IsCurrentAttackReady())
        {
            logicStateMachine.ChangeState(head.attackState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        runningDustEmission.rateOverTime = 0f;
    }
}

public class HeadAttackState : HeadState
{
    public HeadAttackState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        FlipToTarget();
        if (!attackSet.CurrentAttack.IsTargetInAttackArea(targetHandler.CurrentTarget))
        {
            logicStateMachine.ChangeState(head.moveState);
            return;
        }

        attackSet.CurrentAttack.AttackPointLookAtDirection(new Vector2(head.FacingDir, 0));
        animStateMachine.ChangeState(head.animAttackState);
        head.StopVelocity();
    }

    public override void Update()
    {
        base.Update();

        if (head.IsCurrentAnimStateTriggerCalled())
        {
            logicStateMachine.ChangeState(head.moveState);
        }
    }

}
