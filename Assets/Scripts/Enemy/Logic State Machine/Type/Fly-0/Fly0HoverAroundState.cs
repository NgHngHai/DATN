using UnityEngine;

public class Fly0HoverAroundState : Fly0State
{
    Vector2 nextHoverPosition;
    float attackTimer;

    public Fly0HoverAroundState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = fly0.hoverRestTime;
        nextHoverPosition = fly0.transform.position;
    }

    public override void Update()
    {
        attackTimer -= Time.deltaTime;

        if (targetHandler.GetDistanceToTarget() > fly0.hoverAroundDistance)
        {
            logicStateMachine.ChangeState(fly0.chaseState);
        }
        else
        {
            HoveringAroundTarget();
        }
    }

    private void HoveringAroundTarget()
    {
        if (Vector2.Distance(fly0.transform.position, nextHoverPosition) < 0.1f)
        {
            base.Update();

            fly0.StopVelocity();

            DecideHoverOrAttack();
        }
        else
        {
            Vector2 moveDir = (nextHoverPosition - (Vector2)fly0.transform.position).normalized;
            fly0.SetVelocity(moveDir * fly0.moveSpeed);
        }
    }

    private void DecideHoverOrAttack()
    {
        if (stateTimer > 0) return;

        if (attackTimer < 0 && attackBehavior.IsTargetInAttackArea(targetHandler.CurrentTarget))
        {
            attackTimer = fly0.attackCooldown;
            logicStateMachine.ChangeState(fly0.attackState);
        }
        else
        {
            ChooseNextHoverPoint();
        }
    }

    void ChooseNextHoverPoint()
    {
        stateTimer = fly0.hoverRestTime;
        nextHoverPosition = targetHandler.CurrentTarget.position;
        nextHoverPosition += (Random.insideUnitCircle * fly0.hoverAroundDistance);
    }

}
