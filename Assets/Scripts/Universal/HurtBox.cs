using UnityEngine;
using System.Collections.Generic;

public class HurtBox : MonoBehaviour
{
    // Target layer for detecting collisions
    [Header("Targeting")]
    [Tooltip("Layers this hurtbox can affect.")]
    public LayerMask targetLayers = ~0;

    // Damage variables
    [Header("Damage")]
    [Tooltip("How much damage to apply on hit.")]
    public int damageAmount = 1;

    [Tooltip("If true, presentation systems may play a hit reaction on targets.")]
    public bool shouldTriggerHitReaction = true;

    // Knockback variables
    [Header("Knockback")]
    [Tooltip("Apply knockback to the target when damaged.")]
    public bool applyKnockbackToTarget = false;

    [Tooltip("Lock target movement briefly when knockback is applied.")]
    public bool lockMovementOnTarget = false;

    [Tooltip("Duration to lock target movement, in seconds.")]
    public float knockbackLockDuration = 0.1f;

    [Tooltip("Apply knockback to self (recoil) when damaging a target.")]
    public bool applyKnockbackToSelf = false;

    public float targetKnockbackForce = 5f;
    public float selfKnockbackForce = 5f;

    // For player only
    [Header("Player Only")]
    public bool restoreEnergyOnHit = false;
    public int energyRestoredPerHit = 3;

    // Hitting mode
    [Header("Hit Gating")]
    [Tooltip("If true, the same target will be damaged only once while they remain inside.")]
    public bool singleHitPerTarget = true;

    [Tooltip("If > 0 and not singleHitPerTarget, allows re-hitting the same target inside at this interval (seconds).")]
    public float repeatDelay = 0f;

    // References
    // private readonly HashSet<Object> _hitOnce = new HashSet<Object>();
    private readonly Dictionary<Object, float> _lastHitTime = new();
    private Rigidbody2D _selfRb;
    private Collider2D _col;
    private Entity _selfEntity;

    // For restoring energy on hit
    PlayerSkillManager _playerSkillManager;

    private void Awake()
    {
        _col = GetComponent<Collider2D>();
        _selfRb = GetComponentInParent<Rigidbody2D>();
        _selfEntity = GetComponent<Entity>() ?? GetComponentInParent<Entity>();

        if (restoreEnergyOnHit)
        {
            _playerSkillManager = GetComponentInParent<PlayerSkillManager>();
        }
    }

    private void OnEnable()
    {
        _lastHitTime.Clear();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        TryHit(other);
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!singleHitPerTarget && repeatDelay > 0f)
        {
            TryHit(other);
        }
    }

    private void TryHit(Collider2D other)
    {
        if (!IsLayerAllowed(other.gameObject.layer))
            return;

        var damageable = other.GetComponent<IDamageable>();
        Entity entity = other.GetComponent<Entity>();
        if (damageable == null || !damageable.CanBeDamaged())
            return;

        var targetKey = (Object)damageable as Object ?? (Object)other;

        // Gate repeated hits
        if (repeatDelay > 0f)
        {
            if (_lastHitTime.TryGetValue(targetKey, out float last) && (Time.time - last) < repeatDelay)
                return;
        }

        // Apply damage
        bool applied = damageable.TakeDamage(damageAmount, shouldTriggerHitReaction);
        if (!applied)
            return;

        if (repeatDelay > 0f)
            _lastHitTime[targetKey] = Time.time;

        // Knockback
        Vector2 dir = ComputeKnockbackDirection(other);

        if (applyKnockbackToTarget && targetKnockbackForce > 0f)
        {

            var targetEntity = other.GetComponent<Entity>();

            if (targetEntity != null)
                targetEntity.ApplyKnockback(-dir, targetKnockbackForce, lockMovementOnTarget, knockbackLockDuration);
        }

        if (applyKnockbackToSelf && targetKnockbackForce > 0f && _selfRb != null)
        {
            _selfEntity.ApplyKnockback(dir, selfKnockbackForce, false);
        }

        // Restore energy on hit if is player
        if (restoreEnergyOnHit && _playerSkillManager != null)
        {
            if (other.GetComponent<Health>() != null || other.GetComponentInParent<Health>() != null) // Only restore if hitting something with Health
                _playerSkillManager.RestoreEnergyOnAttack(energyRestoredPerHit);
        }
    }

    private bool IsLayerAllowed(int layer)
    {
        return (targetLayers.value & (1 << layer)) != 0;
    }

    private Vector2 ComputeKnockbackDirection(Collider2D target)
    {
        Vector2 dir = gameObject.transform.position - target.transform.position;
        if (dir.sqrMagnitude < 0.0001f) dir = Vector2.right;
        return dir;
    }

    public void ToggleHurtCollider(bool enable)
    {
        _col.enabled = enable;
    }
}
