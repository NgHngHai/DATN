using UnityEngine;
using System.Collections.Generic;

public class HurtBox : MonoBehaviour
{
    [SerializeField] private bool enableColliderAtAwake = true;

    // Target layer for detecting collisions
    [Header("Targeting")]
    [Tooltip("Layers this hurtbox can affect.")]
    public LayerMask targetLayers = ~0;

    // Damage variables
    [Header("Damage")]
    [Tooltip("How much damage to apply on hit.")]
    public int damageAmount = 1;

    [Tooltip("Damage type(s) applied by this hurtbox.")]
    public DamageType damageType = DamageType.Normal;

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
    private readonly Dictionary<Object, float> _lastHitTime = new();
    private Rigidbody2D _selfRb;
    private Collider2D _col;
    private Entity _selfEntity;
    private PlayerController _playerController;
    private SlowMoManager _slowMoManager;

    // For restoring energy on hit
    PlayerSkillManager _playerSkillManager;

    private void Awake()
    {
        _col = GetComponent<Collider2D>();
        _selfRb = GetComponentInParent<Rigidbody2D>();
        _selfEntity = GetComponent<Entity>() ?? GetComponentInParent<Entity>();
        _playerController = GetComponentInParent<PlayerController>();
        _slowMoManager = SlowMoManager.Instance;

        if (restoreEnergyOnHit)
        {
            _playerSkillManager = GetComponentInParent<PlayerSkillManager>();
        }

        _col.enabled = enableColliderAtAwake;
    }

    private void OnEnable()
    {
        _lastHitTime.Clear();
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        TryHit(other);
    }
    protected void OnTriggerStay2D(Collider2D other)
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

        if (repeatDelay > 0f)
            _lastHitTime[targetKey] = Time.time;

        // Knockback
        Vector2 dir = ComputeKnockbackDirection(other);

        if (applyKnockbackToTarget && targetKnockbackForce > 0f)
        {
            var targetEntity = other.GetComponent<Entity>();
            var targetRb = other.GetComponent<Rigidbody2D>();

            if (targetEntity != null)
            {
                targetRb.linearVelocity = Vector2.zero;
                targetEntity.ApplyKnockback(-dir, targetKnockbackForce, lockMovementOnTarget, knockbackLockDuration);
            }
        }

        if (applyKnockbackToSelf && selfKnockbackForce > 0f && _selfRb != null)
        {
            _selfRb.linearVelocity = Vector2.zero;
            _selfEntity.ApplyKnockback(dir, selfKnockbackForce, false);
        }

        // Apply damage
        bool applied = damageable.TakeDamage(damageAmount, damageType, -dir, shouldTriggerHitReaction);
        if (!applied)
            return;

        // Restore energy on hit if is player
        if (restoreEnergyOnHit && _playerSkillManager != null)
        {
            if (other.GetComponent<Health>() != null || other.GetComponentInParent<Health>() != null) // Only restore if hitting something with Health
                _playerSkillManager.RestoreEnergyOnAttack(energyRestoredPerHit);
        }

        // Heavy hit slow-mo (only for player-owned hurtboxes)
        if (_playerController != null && damageType.HasFlag(DamageType.Heavy))
        {
            _slowMoManager.Request();
        }
    }

    private bool IsLayerAllowed(int layer)
    {
        return (targetLayers.value & (1 << layer)) != 0;
    }

    private Vector2 ComputeKnockbackDirection(Collider2D target)
    {
        //Vector2 dir = gameObject.transform.position - target.transform.position;
        //if (dir.sqrMagnitude < 0.0001f) dir = Vector2.right;
        //return dir;
        Vector2 selfPos = transform.position;
        Vector2 targetPos = target.transform.position;

        Vector2 delta = selfPos - targetPos;

        if (delta.sqrMagnitude < 0.0001f)
            return Vector2.right;

        float absX = Mathf.Abs(delta.x);
        float absY = Mathf.Abs(delta.y);

        // Favor vertical knockback for pogo scenarios.
        const float verticalBiasFactor = 0.5f; // smaller -> easier to be considered vertical
        if (absY > absX * verticalBiasFactor)
        {
            return Vector2.up;
        }

        // Otherwise use horizontal
        return delta.x >= 0f ? Vector2.right : Vector2.left;
    }

    public void ToggleHurtCollider(bool enable)
    {
        _col.enabled = enable;
    }
}
