using UnityEngine;

/// <summary>
/// Defines the general structure and flow of an entity’s patrol behavior.
/// Handles transitions between patrol and rest phases, and provides hooks for custom patrol logic.
/// </summary>
public abstract class EnemyPatrolBehavior : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] protected float patrolSpeed;
    [SerializeField] protected float restTime;
    [SerializeField] protected bool patrolAtAwake;

    protected Enemy enemy;
    protected bool isResting;
    protected bool isPatrolProcessStopped = false;
    protected float restTimer;

    protected virtual void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    private void Start()
    {
        if (patrolAtAwake)
            StartPatrolProcess();
        else
            StopPatrolProcess();
    }

    private void Update()
    {
        if (isPatrolProcessStopped) return;

        if (isResting)
            Resting();
        else
            Patrolling();
    }

    protected virtual void EnterRest()
    {
        isResting = true;
        restTimer = restTime;
        enemy.StopVelocity();
    }

    protected virtual void Resting()
    {
        restTimer -= Time.deltaTime;
        if (restTimer < 0)
        {
            EnterPatrol();
        }
    }

    protected virtual void EnterPatrol()
    {
        isResting = false;
    }

    protected abstract void Patrolling();

    public void StartPatrolProcess()
    {
        isPatrolProcessStopped = false;
        EnterRest();
    }

    public void StopPatrolProcess()
    {
        isPatrolProcessStopped = true;
    }
}
