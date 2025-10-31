using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class Health : MonoBehaviour, IDamageable
{
    [System.Serializable]
    public class HealthChangedEvent : UnityEvent<int, int> { } // (currentHealth, maxHealth)

    [System.Serializable]
    public class DamagedWithReactionEvent : UnityEvent<int, bool> { } // (appliedAmount, shouldTriggerHitReaction)

    [Header("Health Settings")]
    [Tooltip("Maximum health value.")]
    public int maxHealth = 100;

    [Tooltip("Current health. Clamped to [0, maxHealth].")]
    public int currentHealth = 100;

    [Tooltip("When true, incoming damage is ignored.")]
    public bool isInvincible = false;

    [Header("Events")]
    [Tooltip("Invoked when the component takes damage with reaction info. Parameters: amount applied, shouldTriggerHitReaction.")]
    public DamagedWithReactionEvent OnDamagedWithReaction = new DamagedWithReactionEvent();

    [Tooltip("Invoked when the component is healed. Parameter: amount healed.")]
    public UnityEvent<int> OnHealed = new UnityEvent<int>();

    [Tooltip("Invoked when health changes. Parameters: currentHealth, maxHealth.")]
    public HealthChangedEvent OnHealthChanged = new HealthChangedEvent();

    [Tooltip("Invoked when health reaches zero.")]
    public UnityEvent OnDeath = new UnityEvent();

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
    }

    /// Apply damage. Returns true if damage was applied (not invincible and amount > 0).
    /// New parameter: shouldTriggerHitReaction — caller indicates whether presentation systems should treat this as a "hit" reaction.
    /// Defaults to true for backward-compatible behavior.
    public bool TakeDamage(int amount, bool shouldTriggerHitReaction = true)
    {
        if (amount <= 0) return false;
        if (isInvincible) return false;
        if (currentHealth <= 0) return false; // already dead

        int prev = currentHealth;
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        int applied = prev - currentHealth;

        if (applied > 0)
        {
            // new event includes the reaction hint
            OnDamagedWithReaction?.Invoke(applied, shouldTriggerHitReaction);

            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            if (currentHealth == 0)
                OnDeath?.Invoke();
            return true;
        }

        return false;
    }

    /// Returns whether this component can currently be damaged (not invincible and not already dead).
    public bool CanBeDamaged()
    {
        return !isInvincible && currentHealth > 0;
    }

    /// Heal the entity. Returns true if any healing occurred.
    public bool Heal(int amount)
    {
        if (amount <= 0) return false;
        if (currentHealth <= 0) return false; // optionally prevent healing a dead entity; remove check if desired

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

    /// Sets max health. Optionally adjust current health to stay within new max.
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

    /// Fully restore health to max.
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

    /// Returns whether the entity is dead (health == 0).
    public bool IsDead() => currentHealth <= 0;
}