using System;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class Health : MonoBehaviour, IDamageable
{
    [System.Serializable]
    public class HealthChangedEvent : UnityEvent<int, int> { } // (currentHealth, maxHealth)

    [System.Serializable]
    public class DamagedWithReactionEvent : UnityEvent<int, Vector2, bool> { } // (appliedAmount, HitDirection ,shouldTriggerHitReaction)

    [Header("Health Settings")]
    [Tooltip("Maximum health value.")]
    public int maxHealth = 100;

    [Tooltip("Current health. Clamped to [0, maxHealth].")]
    public int currentHealth = 100;

    [Header("Damage Handling")]
    [Tooltip("Which damage types this entity can receive")]
    public DamageType acceptedDamageTypes = DamageType.Normal;


    [Tooltip("When true, incoming damage is ignored.")]
    public bool isInvincible = false;

    public float damageMultiplier = 1f;

    [SerializeField] private float iframeTime = 0.3f;
    // Timestamp (in Time.time) until which i-frames are active after taking damage.
    private float _iFrameUntilTime = 0f;

    [Header("Damage Types")]
    [Tooltip("Damage of these types will ignore i-frames.")]
    [SerializeField] private DamageType damageTypesIgnoreIFrames = DamageType.Poison;

    [Header("Events")]
    [Tooltip("Invoked when the component takes damage with reaction info. Parameters: amount applied, shouldTriggerHitReaction.")]
    public DamagedWithReactionEvent OnDamagedWithReaction = new();

    [Tooltip("Invoked when the component is healed. Parameter: amount healed.")]
    public UnityEvent<int> OnHealed = new();

    [Tooltip("Invoked when health changes. Parameters: currentHealth, maxHealth.")]
    public HealthChangedEvent OnHealthChanged = new();

    [Tooltip("Invoked when health reaches zero.")]
    public UnityEvent OnDeath = new();

    private void Reset()
    {
        // Keep sensible defaults when added in inspector
        maxHealth = 100;
        currentHealth = maxHealth;
        isInvincible = false;
    }

    private void Awake()
    {
        // Ensure currentHealth is valid on start
        currentHealth = Mathf.Clamp(currentHealth, 0, Mathf.Max(1, maxHealth));
        _iFrameUntilTime = 0f;
    }

    public bool AcceptDamageType(DamageType incoming)
    {
        return (acceptedDamageTypes & incoming) != 0;
    }

    /// Apply damage. Returns true if damage was applied (not invincible and amount > 0).
    public bool TakeDamage(int amount, DamageType type, Vector2 hitDir, bool shouldTriggerHitReaction = true)
    {
        if (amount <= 0) return false;
        if (!CanBeDamaged() && !DamageTypeIgnoresIFrames(type)) return false;
        if (currentHealth <= 0) return false; // already dead
        if (!AcceptDamageType(type)) return false;

        int prev = currentHealth;
        currentHealth = (int)Mathf.Clamp(currentHealth - MathF.Round(amount * damageMultiplier), 0, maxHealth);
        int applied = prev - currentHealth;

        if (applied > 0)
        {
            // Start/refresh iframes after applying damage
            if (iframeTime > 0f) // (iframeTime > 0f && type != DamageType.Poison)
                _iFrameUntilTime = Mathf.Max(_iFrameUntilTime, Time.time + iframeTime);

            // new event includes the reaction hint
            OnDamagedWithReaction?.Invoke(applied, hitDir, shouldTriggerHitReaction);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            if (currentHealth == 0)
                OnDeath?.Invoke();
            return true;
        }

        return false;
    }

    // Returns whether this component can currently be damaged (not invincible and not already dead).
    public bool CanBeDamaged()
    {
        return !isInvincible && !IsInIFrame() && currentHealth > 0;
    }

    private bool DamageTypeIgnoresIFrames(DamageType type)
    {
        return (type & damageTypesIgnoreIFrames) != 0;
    }

    // True if currently inside the invincibility window granted after taking damage.
    public bool IsInIFrame()
    {
        return Time.time < _iFrameUntilTime;
    }

    // Heal the entity. Returns true if any healing occurred.
    public bool Heal(int amount)
    {
        if (amount <= 0) return false;
        if (currentHealth <= 0) return false; // Prevent healing a dead entity

        int prev = currentHealth;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        int healed = currentHealth - prev;

        if (healed > 0)
        {
            OnHealed?.Invoke(healed);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            return true;
        }

        return false;
    }

    // Sets max health. Optionally adjust current health to stay within new max.
    public void SetMaxHealth(int newMax, bool clampCurrentToMax = true)
    {
        maxHealth = Mathf.Max(1, newMax);
        if (clampCurrentToMax)
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void SetInvincible(bool inv)
    {
        isInvincible = inv;
    }

    // Fully restore health to max.
    public void RestoreToFull()
    {
        int prev = currentHealth;
        currentHealth = maxHealth;
        int healed = currentHealth - prev;
        if (healed > 0)
        {
            OnHealed?.Invoke(healed);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
    }

    // Returns whether the entity is dead (health == 0).
    public bool IsDead() => currentHealth <= 0;
}