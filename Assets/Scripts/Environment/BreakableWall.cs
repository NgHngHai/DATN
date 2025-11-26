using System.Runtime.CompilerServices;
using UnityEngine;

public class BreakableWall : MonoBehaviour, IDamageable, ISaveable
{
    public int health = 5;
    [SerializeField] private string uniqueID;
    private bool isDestroyed = false;

    public bool TakeDamage (int amount, bool trigger = false)
    {
        if (isDestroyed) return false;

        health--;

        if (health == 0)
        {
            gameObject.SetActive(false);
        }

        return true;
    }

    public bool CanBeDamaged() => !isDestroyed;
    public bool IsDead() => isDestroyed;

    // -----SAVE SYSTEM-----
    public string GetUniqueID() => uniqueID;

    public object CaptureState()
    {
        return new WallState
        {
            destroyed = isDestroyed,
            position = transform.position
        };
    }

    public void RestoreState(object state)
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
