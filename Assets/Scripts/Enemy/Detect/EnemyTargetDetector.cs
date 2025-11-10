using UnityEngine;

/// <summary>
/// Base class for enemy target detection logic.
/// Periodically scans for visible targets and assigns them to the <see cref="EnemyTargetHandler"/>.
/// </summary>
[RequireComponent(typeof(EnemyTargetHandler))]
public abstract class EnemyTargetDetector : MonoBehaviour
{
    [Header("Base Properties")]
    [SerializeField] protected LayerMask targetMask;
    [SerializeField] protected LayerMask obstacleMask;
    [SerializeField] protected float detectDistance = 5f;
    [SerializeField] protected Transform detectPoint;
    [SerializeField] protected float detectRate = 0.2f;
    [SerializeField] protected bool drawGizmos;

    protected Enemy enemy;
    protected EnemyTargetHandler targetHandler;
    protected float detectTimer;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        targetHandler = GetComponent<EnemyTargetHandler>();
    }

    protected virtual void Update()
    {
        detectTimer -= Time.deltaTime;
        if (detectTimer <= 0)
        {
            detectTimer = detectRate;
            DetectTarget();
        }
    }

    private void DetectTarget()
    {
        if (targetHandler == null) return;

        Transform detected = GetFirstDetectedTarget();

        if (detected != null)
        {
            targetHandler.SetTarget(detected);
        }
    }

    protected Vector2 GetFaceDirection()
    {
        if (enemy == null) return Vector2.right;
        return new Vector2(enemy.FacingDir, 0);
    }

    protected abstract Transform GetFirstDetectedTarget();

    protected virtual void OnDrawGizmos()
    {
        if (!drawGizmos || detectPoint == null) return;

        Color visionColor = Color.cyan;
        visionColor.a = 0.3f;
        Gizmos.color = visionColor;
    }
}
