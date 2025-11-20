using UnityEngine;

public class EnemyIncreaseAttackSpeedAura : MonoBehaviour
{
    [SerializeField] private float activateInterval = 0.3f; 
    [SerializeField] private float increasePercent = 0.5f;
    [SerializeField] private float effectTime = 0.5f;
    [SerializeField] private float effectRadius = 4f;
    [SerializeField] private LayerMask enemyMask;

    private Enemy enemyItself;
    private float activateTimer;
    private bool isAuraStopped;

    private void Awake()
    {
        enemyItself = GetComponent<Enemy>();
    }

    private void Update()
    {
        if (isAuraStopped) return;

        activateTimer -= Time.deltaTime;
        if(activateTimer < 0)
        {
            IncreaseAttackSpeed();
        }
    }

    private void IncreaseAttackSpeed()
    {
        activateTimer = activateInterval;

        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, effectRadius, enemyMask);

        foreach (var col in cols)
        {
            Enemy e = col.GetComponent<Enemy>();
            if (e == null || e == enemyItself) continue;

            e.AttackSet.IncreaseAttackSpeed(increasePercent, effectTime);
        }
    }

    public void StopAura()
    {
        isAuraStopped = true;
        activateTimer = 0;
    }

    public void StartAura()
    {
        isAuraStopped = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.orangeRed;
        Gizmos.DrawWireSphere(transform.position, effectRadius);
    }
}
