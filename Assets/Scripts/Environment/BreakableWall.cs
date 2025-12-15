using UnityEngine;
using System.Collections;

public class BreakableWall : SaveableObject, IDamageable
{
    public int health = 5;
    [SerializeField] private float destroyAnimationDuration = 0.5f;
    [SerializeField] private GameObject wallComponents;
    [SerializeField] private ParticleSystem destroyParticles;
    private bool isDestroyed = false;

    public bool TakeDamage (int amount, DamageType type, Vector2 hitDir, bool trigger = false)
    {
        if (isDestroyed) return false;

        if ((type & DamageType.Heavy) == 0)
        {
            // Only heavy damage can damage the wall
            return false;
        }

        health--;

        if (health == 0)
        {
            isDestroyed = true;
            StartCoroutine(DestroyCoroutine());
        }

        return true;
    }

    private IEnumerator DestroyCoroutine()
    {
        wallComponents.SetActive(false);
        destroyParticles.Play();

        while (destroyParticles.isPlaying)
        {
            yield return null;
        }

        gameObject.SetActive(false);
    }

    public bool CanBeDamaged() => !isDestroyed;
    public bool IsDead() => isDestroyed;

    // -----SAVE SYSTEM-----
    public override object CaptureState()
    {
        return new WallState
        {
            destroyed = isDestroyed,
        };
    }

    public override void RestoreState(object state)
    {
        var s = (WallState)state;
        isDestroyed = s.destroyed;

        // Unload if destroyed
        gameObject.SetActive(!isDestroyed);
    }

    [System.Serializable]
    private struct WallState
    {
        public bool destroyed;
    }
}
