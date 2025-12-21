using UnityEngine;
using UnityEngine.Events;

public class TrapHurtBox : HurtBox
{
    [Header("Trap Settings")]
    [Tooltip("If true, player velocity is zeroed on teleport.")]
    public bool zeroVelocityOnTeleport = true;

    [Header("Enemy Filter")]
    [Tooltip("Any collider on these layers will be instantly killed on contact.")]
    [SerializeField] private LayerMask enemyLayers;

    [Header("Trap Events")]
    [Tooltip("Invoked when the player is hit by the trap.")]
    public UnityEvent OnTrapOverlayRequested;

    private RoomData _roomData;

    protected void Awake()
    {
        // Cache the room data from the trap's hierarchy (room instance)
        _roomData = GetComponentInParent<RoomData>();
        if (_roomData == null)
        {
            Debug.LogWarning("TrapHurtBox: RoomData not found in parent. Teleport will be skipped.");
        }
    }

    new private void OnTriggerEnter2D(Collider2D other)
    {
        // Perform normal hurt behavior
        base.OnTriggerEnter2D(other);

        // Kill enemies instantly if they are on chosen layers
        if (IsEnemyCollider(other))
        {
            KillEnemy(other);
        }

        // Teleport player to room spawn point
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            var health = player != null ? (player.playerHealth ?? player.GetComponent<Health>()) : null;

            // If lethal, let PlayerRespawnController handle overlay + full reset
            if (health != null && health.IsDead())
                return;

            // Non-lethal: teleport only (no revive animation here)
            TeleportPlayerToRoomSpawn(player);
        }
    }

    new private void OnTriggerStay2D(Collider2D other)
    {
        // Preserve repeat hit behavior from base
        base.OnTriggerStay2D(other);

        if (other.CompareTag("Player") && ShouldTeleportOnStay())
        {
            var player = other.GetComponent<PlayerController>();
            var health = player != null ? (player.playerHealth ?? player.GetComponent<Health>()) : null;

            // If lethal, do nothing
            if (health != null && health.IsDead())
                return;

            TeleportPlayerToRoomSpawn(player);
        }
    }

    private bool ShouldTeleportOnStay()
    {
        // Only allow repeated teleports if base is set to repeat hits
        return !singleHitPerTarget && repeatDelay > 0f;
    }

    private void TeleportPlayerToRoomSpawn(PlayerController player)
    {
        if (player == null || _roomData == null)
            return;

        player.transform.position = _roomData.FirstSpawnPosition;
        if (zeroVelocityOnTeleport && player.TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    private bool IsEnemyCollider(Collider2D other)
    {
        int bit = 1 << other.gameObject.layer;
        return (enemyLayers.value & bit) != 0;
    }

    private void KillEnemy(Collider2D other)
    {
        // Prefer Health to guarantee a kill regardless of i-frames
        var health = other.GetComponent<Health>() ?? other.GetComponentInParent<Health>();
        if (health != null)
        {
            ForceKill(health);
            return;
        }

        // Fallback to generic IDamageable
        var dmg = other.GetComponent<IDamageable>() ?? other.GetComponentInParent<IDamageable>();
        if (dmg != null && !dmg.IsDead())
        {
            dmg.TakeDamage(int.MaxValue, DamageType.Normal, Vector2.up, false);
        }
    }

    private static void ForceKill(Health h)
    {
        if (h == null || h.IsDead()) return;

        h.isInvincible = false;
        h.currentHealth = 0;
        h.OnHealthChanged?.Invoke(h.currentHealth, h.maxHealth);
        h.OnDeath?.Invoke();
    }
}