using UnityEngine;

public class EnemyShield : MonoBehaviour
{
    [SerializeField] private GameObject animObject;
    [SerializeField] private float shieldCooldown;
    [SerializeField] private Health enemyHealth;

    public bool CanRegenerate;

    private bool isActive;
    private DamageType baseDamageType;
    private float cooldownTimer;

    private void Awake()
    {
        if(enemyHealth == null) 
            enemyHealth = GetComponentInParent<Health>();

        baseDamageType = enemyHealth.acceptedDamageTypes;
        TryActivate();
    }

    private void Update()
    {
        if (!CanRegenerate) return;

        cooldownTimer -= Time.deltaTime;
    }

    private void OnEnable()
    {
        if (enemyHealth != null)
            enemyHealth.OnDamagedWithReaction.AddListener(OnOwnerDamaged);
    }

    private void OnDisable()
    {
        if (enemyHealth != null)
            enemyHealth.OnDamagedWithReaction.RemoveListener(OnOwnerDamaged);
    }

    private void OnOwnerDamaged(int damage, Vector2 hitDir, bool shouldTriggerHitReaction)
    {
        Deativate();
    }

    public void TryActivate()
    {
        if (cooldownTimer > 0) return;

        enemyHealth.acceptedDamageTypes = DamageType.Counter;
        animObject.SetActive(true);
        isActive = true;
    }

    private void Deativate()
    {
        if (!isActive) return;

        cooldownTimer = shieldCooldown;

        enemyHealth.acceptedDamageTypes = baseDamageType;
        animObject.SetActive(false);
        isActive = false;
    }
}
