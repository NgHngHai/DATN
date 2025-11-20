using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

public class L4SIState : EnemyOffensiveState
{
    protected EnemyL4SI l4si;
    public L4SIState(Enemy enemy) : base(enemy)
    {
        l4si = enemy as EnemyL4SI;
    }
}

public class L4SISupportState : L4SIState
{
    private Health health;
    private bool isFirstTimeAttack;

    public L4SISupportState(Enemy enemy) : base(enemy)
    {
        health = l4si.GetComponent<Health>();
    }

    public override void Enter()
    {
        base.Enter();
        l4si.increaseAttackSpeedAura.StartAura();
        health.OnDamagedWithReaction.AddListener(OnDamageReaction);
        stateTimer = isFirstTimeAttack ? 0 : l4si.attackModeCooldown;
    }

    public override void Exit()
    {
        base.Exit();
        l4si.increaseAttackSpeedAura.StopAura();
        health.OnDamagedWithReaction.RemoveListener(OnDamageReaction);
    }

    private void OnDamageReaction(int appliedAmount, bool shouldTriggerHitReaction)
    {
        logicStateMachine.ChangeState(l4si.wingAttackState);
    }
}

public class L4SIWingAttackState : L4SIState
{
    private int currentCount;
    public L4SIWingAttackState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        currentCount = 0;
    }

    public override void Update()
    {
        base.Update();
        if (currentCount < l4si.throwWingCount)
        {
            if (IsTargetValid())
                attackSet.CurrentAttack.AttackPointLookAt(targetHandler.CurrentTarget);

            if (IsCurrentAttackReady())
            {
                TryCurrentAttack();
                currentCount++;
            }
                
        }
        else
        {
            logicStateMachine.ChangeState(l4si.burrowAttackState);
        }
    }
}

public class L4SIBurrowAttackState : L4SIState
{
    private Vector2 endPosition;
    private Vector2 startPosition;

    private float checkBurrowDistance = 100;
    private float burrowDistance = 3;
    private float curveDuration;

    public L4SIBurrowAttackState(Enemy enemy) : base(enemy)
    {
        curveDuration = l4si.burrowAttackDuration;
    }

    public override void Enter()
    {
        base.Enter();

        l4si.burrowHurtBox.ToggleHurtCollider(true);
        CaculatedStartEndPosition();
        stateTimer = curveDuration;
    }

    public override void Update()
    {
        base.Update();

        float t = l4si.burrowCurve.Evaluate((curveDuration - stateTimer) / curveDuration); 
        l4si.transform.position = Vector2.Lerp(startPosition, endPosition, t);

        if (stateTimer < 0)
            logicStateMachine.ChangeState(l4si.teleportAwayState);
    }

    public override void Exit()
    {
        base.Exit();
        l4si.burrowHurtBox.ToggleHurtCollider(false);
    }

    private void CaculatedStartEndPosition()
    {
        if (IsTargetValid())
            endPosition = targetHandler.CurrentTarget.position;

        RaycastHit2D hit = Physics2D.Raycast(endPosition, Vector2.down, checkBurrowDistance, l4si.burrowMask);

        if (hit.collider != null)
        {
            startPosition = hit.point;
            startPosition.y -= burrowDistance;
        }
    }
}

public class L4SITeleportAwayState : L4SIState
{
    public L4SITeleportAwayState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = l4si.teleportDelay;
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
        {
            l4si.TeleportToClosetEnemy();
            logicStateMachine.ChangeState(l4si.supportState);
        }
    }

}

