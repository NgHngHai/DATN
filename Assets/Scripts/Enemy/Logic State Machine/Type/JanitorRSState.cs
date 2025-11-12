using UnityEngine;

public class JanitorRSState : EnemyState
{
    protected EnemyJanitorRS janitorRS;

    public JanitorRSState(Enemy enemy) : base(enemy)
    {
        janitorRS = enemy as EnemyJanitorRS;
    }
}

public class JanitorRSIdleState : JanitorRSState
{
    private Health health;
    private bool isFirstTimeTeleport = true;

    public JanitorRSIdleState(Enemy enemy) : base(enemy)
    {
        health = janitorRS.GetComponent<Health>();
    }

    public override void Enter()
    {
        base.Enter();

        health.OnDamagedWithReaction.AddListener(OnDamageTeleport);

        if (isFirstTimeTeleport) return;

        stateTimer = janitorRS.teleportCooldown;
    }

    public override void Exit()
    {
        base.Exit();
        health.OnDamagedWithReaction.RemoveListener(OnDamageTeleport);
    }

    private void OnDamageTeleport(int appliedAmount, bool shouldTriggerHitReaction)
    {
        if (stateTimer > 0) return;
        isFirstTimeTeleport = false;
        logicStateMachine.ChangeState(janitorRS.teleportState);
    }
}

public class JanitorRSTeleportState : JanitorRSState
{
    private bool onBaseA = true;

    public JanitorRSTeleportState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = janitorRS.teleportDelay;
    }

    public override void Update()
    {
        base.Update();
        if (stateTimer < 0)
        {
            Vector2 teleportPosition = onBaseA ? janitorRS.teleportBaseB.position : janitorRS.teleportBaseA.position;
            teleportPosition.y += janitorRS.teleportOffsetY;
            janitorRS.transform.position = teleportPosition;

            onBaseA = !onBaseA;

            logicStateMachine.ChangeState(janitorRS.idleState);
        }
    }
}