using UnityEngine;

/// <summary>
/// Ground enemy that only patrols and releases poisonous smoke after a delay.
/// Poison smoke deals damage over time.
/// </summary>
public class EnemyRoak : GroundEnemy
{
    [Header("Enemy: Roak")]
    [Tooltip("Time interval between each poison smoke release.")]
    public float createPoisonDelay;

    [Tooltip("Prefab of the poison smoke created periodically.")]
    [SerializeField] private GameObject poisonSmoke;

    [Tooltip("Layer mask defining which objects can be damaged by contact.")]
    [SerializeField] private LayerMask damageMask;

    [Tooltip("Amount of damage applied when touching valid targets.")]
    [SerializeField] private int damageOnContact = 3;

    public RoakState mainState;

    protected override void Awake()
    {
        base.Awake();
        mainState = new RoakState(this);
    }

    protected override void Start()
    {
        base.Start();
        logicStateMachine.Initialize(mainState);
    }

    public void CreatePoison()
    {
        Instantiate(poisonSmoke, transform.position, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (PhysicsUtils.IsGameObjectInLayer(collision.gameObject, damageMask))
        {
            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damageOnContact);
            }
        }
    }

}
