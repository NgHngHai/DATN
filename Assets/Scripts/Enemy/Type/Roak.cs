using UnityEngine;

/// <summary>
/// Ground enemy that only patrols and releases poisonous smoke after a delay.
/// Poison smoke deals damage over time.
/// </summary>
public class Roak : GroundEnemy
{
    [Header("Roak")]
    public float createPoisonDelay;
    [SerializeField] private GameObject poisonSmoke;
    [SerializeField] private LayerMask whatCanBeDamaged;
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
        if (PhysicsUtils.IsGameObjectInLayer(collision.gameObject, whatCanBeDamaged))
        {
            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damageOnContact);
            }
        }
    }

}
