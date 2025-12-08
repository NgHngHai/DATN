using UnityEngine;

public class BreakableWall : SaveableObject, IDamageable
{
    public int health = 5;
    [SerializeField] private string uniqueID;
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
            gameObject.SetActive(false);
        }

        return true;
    }

    public bool CanBeDamaged() => !isDestroyed;
    public bool IsDead() => isDestroyed;

    // -----SAVE SYSTEM-----
    public override object CaptureState()
    {
        return new WallState
        {
            destroyed = isDestroyed,
            position = transform.position
        };
    }

    public override void RestoreState(object state)
    {
        var s = (WallState)state;
        isDestroyed = s.destroyed;
        transform.position = s.position;

        // Unload if destroyed
        gameObject.SetActive(!isDestroyed);
    }

    [System.Serializable]
    private struct WallState
    {
        public bool destroyed;
        public Vector3 position;
    }
}
