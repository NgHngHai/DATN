using UnityEngine;

/// <summary>
/// Base class for entity target detection logic.
/// Periodically scans for visible targets and assigns them to the <see cref="EnemyTargetHandler"/>.
/// </summary>
[RequireComponent(typeof(EnemyTargetHandler))]
public abstract class EnemyTargetDetector : MonoBehaviour
{
    [Header("Base Properties")]
    [Tooltip("Layer(s) that represent potential targets")]
    [SerializeField] protected LayerMask targetMask;

    [Tooltip("Layer(s) that block the enemy’s line of sight")]
    [SerializeField] protected LayerMask obstacleMask;

    [Tooltip("Maximum detection distance from the detect point ")]
    [SerializeField] protected float detectDistance = 5f;

    [Tooltip("Transform used as the origin point for detection")]
    [SerializeField] protected Transform detectPoint;

    [Tooltip("Time interval (seconds) between each detection scan.")]
    [SerializeField] protected float detectRate = 0.2f;

    [Tooltip("If true, draws debug Gizmos in the Scene view for visualization.")]
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

    protected abstract Transform GetFirstDetectedTarget();

    protected void OnDrawGizmosSelected()
    {
        if (!drawGizmos || detectPoint == null) return;

        DrawGizmos();
    }

    protected virtual void DrawGizmos()
    {
        Color visionColor = Color.cyan;
        visionColor.a = 0.3f;
        Gizmos.color = visionColor;
    }
}
