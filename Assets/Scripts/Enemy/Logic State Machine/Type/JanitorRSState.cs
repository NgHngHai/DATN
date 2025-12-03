using System;
using UnityEngine;

public class JanitorRSState : EnemyState
{
    protected EnemyJanitorRS janitorRS;

    protected bool onBaseA;

    private Animator baseA_Animator;
    private Animator baseB_Animator;


    public JanitorRSState(Enemy enemy) : base(enemy)
    {
        janitorRS = enemy as EnemyJanitorRS;

        baseA_Animator = janitorRS.teleportBaseA.GetComponentInChildren<Animator>();
        baseB_Animator = janitorRS.teleportBaseB.GetComponentInChildren<Animator>();
    }


    protected void PlayEquipAnimForCurrentBase(bool isEquipped)
    {
        if (onBaseA)
            baseA_Animator.SetBool("isEquipped", isEquipped);
        else
            baseB_Animator.SetBool("isEquipped", isEquipped);
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

    private void OnDamageTeleport(int appliedAmount, Vector2 hitDir, bool shouldTriggerHitReaction)
    {
        if (stateTimer > 0) return;
        isFirstTimeTeleport = false;
        logicStateMachine.ChangeState(janitorRS.teleportState);
        if (health.IsDead())
        {
            PlayEquipAnimForCurrentBase(false);
        }
    }
}

public class JanitorRSTeleportState : JanitorRSState
{
    private Vector2 baseB_Position;
    private Vector2 baseA_Position;
    private bool alreadyTeleport;

    public JanitorRSTeleportState(Enemy enemy) : base(enemy)
    {
        baseA_Position = janitorRS.teleportBaseA.position;
        baseB_Position = janitorRS.teleportBaseB.position;
    }

    public override void Enter()
    {
        base.Enter();
        InitializeTeleport();
    }

    private void InitializeTeleport()
    {
        alreadyTeleport = false;
        animStateMachine.ChangeState(janitorRS.animRunState);
        PlayEquipAnimForCurrentBase(false);
    }

    public override void Update()
    {
        base.Update();

        if (!janitorRS.IsCurrentAnimStateTriggerCalled()) return;

        // If run animation trigger is called, start teleport to other base.
        // Else if spawn animation trigger is called, change to idle state.
        if (!alreadyTeleport)
        {
            TeleportToOtherBase();
        }
        else
        {
            animStateMachine.ChangeState(janitorRS.animIdleState);
            logicStateMachine.ChangeState(janitorRS.idleState);
        }
    }

    protected void TeleportToOtherBase()
    {
        onBaseA = !onBaseA;
        janitorRS.transform.position = onBaseA ? baseA_Position : baseB_Position;
        PlayEquipAnimForCurrentBase(true);
        animStateMachine.ChangeState(janitorRS.animSpawnState);
        alreadyTeleport = true;
    }

}