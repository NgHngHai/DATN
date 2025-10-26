using UnityEngine;

/// <summary>
/// Manages the enemy's current target.
/// Handles target assignment, loss of vision, and forgetting logic over time.
/// </summary>
public class EnemyTargetHandler : MonoBehaviour
{
    public Transform CurrentTarget { get; private set; }

    [Header("Target Settings")]
    [SerializeField] private float lostVisionOfTargetDistance = 5f;
    [SerializeField] private float forgetTargetDelay = 2f; 

    private float forgetTimer;

    public bool HasTarget => CurrentTarget != null;

    private void Update()
    {
        if (CurrentTarget == null) return;

        float distance = Vector2.Distance(transform.position, CurrentTarget.position);

        if (distance > lostVisionOfTargetDistance)
        {
            forgetTimer -= Time.deltaTime;
            if (forgetTimer <= 0)
                ClearTarget();
        }
        else
        {
            forgetTimer = forgetTargetDelay;
        }
    }

    public void SetTarget(Transform target)
    {
        if (target == null || target == CurrentTarget) return;

        CurrentTarget = target;
        forgetTimer = forgetTargetDelay;
    }

    public void ClearTarget()
    {
        if (CurrentTarget == null) return;
        CurrentTarget = null;
    }

    public Vector2 GetDirectionToTarget()
    {
        if (CurrentTarget == null) return Vector2.zero;
        return (CurrentTarget.position - transform.position).normalized;
    }

    public float GetDistanceToTarget()
    {
        if (CurrentTarget == null) return Mathf.Infinity;
        return Vector2.Distance(transform.position, CurrentTarget.position);
    }

    public int GetTargetFacingDir()
    {
        if (CurrentTarget == null) return 0;
        return (CurrentTarget.position.x - transform.position.x > 0) ? 1 : -1;
    }

    public bool IsCurrentTargetNotNull() => CurrentTarget != null;

    private void OnDrawGizmos()
    {
        Color visionColor = Color.darkCyan;
        visionColor.a = 0.1f;
        Gizmos.color = visionColor;

        Gizmos.DrawWireSphere(transform.position, lostVisionOfTargetDistance);
    }
}
