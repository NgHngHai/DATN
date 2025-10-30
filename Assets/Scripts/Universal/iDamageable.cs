using UnityEngine;

/// Minimal interface for things that can be damaged or hit.
/// Implementers can use this for players, enemies, breakable objects, walls, etc.

public interface iDamageable
{
    /// Apply damage. Returns true if damage was applied.
    bool TakeDamage(int amount, bool shouldTriggerHitReaction = true);

    /// Returns whether this component can currently be damaged (for example, not invincible or already destroyed).
    bool CanBeDamaged();

    /// Returns whether the entity is dead/destroyed.
    bool IsDead();
}