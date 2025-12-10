using UnityEngine;

public class EnemyShield : MonoBehaviour
{
    [SerializeField] private GameObject animObject;
    [SerializeField] private float shieldCooldown;

    public bool CanRegenerate;

    private bool isActive;
    private DamageType baseDamageType;
    private Health health;
    private float cooldownTimer;

    private void Update()
    {
        if (!CanRegenerate) return;

        cooldownTimer -= Time.deltaTime;
    }

    private void OnEnable()
    {
        if (health != null)
            health.OnDamagedWithReaction.AddListener(OnOwnerDamaged);
    }

    private void OnDisable()
    {
        if (health != null)
            health.OnDamagedWithReaction.RemoveListener(OnOwnerDamaged);
    }

    private void OnOwnerDamaged(int damage, Vector2 hitDir, bool shouldTriggerHitReaction)
    {
        Deativate();
    }

    public void TryActivate()
    {
        if(health == null)
        {
            health = GetComponentInParent<Health>();
            baseDamageType = health.acceptedDamageTypes;
            health.OnDamagedWithReaction.AddListener(OnOwnerDamaged);
        }

        if (cooldownTimer > 0) return;

        health.acceptedDamageTypes = DamageType.Counter;

        animObject.SetActive(true);
        isActive = true;
    }

    private void Deativate()
    {
        if (!isActive) return;

        cooldownTimer = shieldCooldown;
        health.acceptedDamageTypes = baseDamageType;
        animObject.SetActive(false);
        isActive = false;
    }
}
