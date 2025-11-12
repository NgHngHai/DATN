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

    [Tooltip("Apply knockback to self (recoil) when damaging a target.")]
    public bool applyKnockbackToSelf = false;

    [Tooltip("Force magnitude for knockback.")]
    public float knockbackForce = 5f;
    public ForceMode2D knockbackForceMode = ForceMode2D.Impulse;

    // Hitting mode
    [Header("Hit Gating")]
    [Tooltip("If true, the same target will be damaged only once while they remain inside.")]
    public bool singleHitPerTarget = true;

    [Tooltip("If > 0 and not singleHitPerTarget, allows re-hitting the same target inside at this interval (seconds).")]
    public float repeatDelay = 0f;

    // References
    private readonly HashSet<Object> _hitOnce = new HashSet<Object>();
    private readonly Dictionary<Object, float> _lastHitTime = new Dictionary<Object, float>();
    private Rigidbody2D _selfRb;
    private Collider2D _col;

    private void Awake()
    {
        _col = GetComponent<Collider2D>();
        _selfRb = GetComponentInParent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        _hitOnce.Clear();
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
        if (singleHitPerTarget)
        {
            if (_hitOnce.Contains(targetKey))
                return;
        }
        else if (repeatDelay > 0f)
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

        // Knockback direction is the component's positive X (transform.right)
        Vector2 dir = ComputeKnockbackDirection();

        if (applyKnockbackToTarget && knockbackForce > 0f)
        {
            var targetRb = other.attachedRigidbody ?? other.GetComponentInParent<Rigidbody2D>();
            Debug.Log("HurtBox applying knockback to target: " + targetRb);
            if (targetRb != null)
                targetRb.AddForce(dir * knockbackForce, knockbackForceMode);
        }

        if (applyKnockbackToSelf && knockbackForce > 0f && _selfRb != null)
        {
            _selfRb.AddForce(-dir * knockbackForce, knockbackForceMode);
        }
    }

    private bool IsLayerAllowed(int layer)
    {
        return (targetLayers.value & (1 << layer)) != 0;
    }

    private Vector2 ComputeKnockbackDirection()
    {
        Vector2 dir = transform.right;
        if (dir.sqrMagnitude < 0.0001f) dir = Vector2.right;
        return dir;
    }

    public void ToggleHurtCollider(bool enable)
    {
        _col.enabled = enable;
    }
}
