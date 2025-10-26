using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Deals continuous damage to all entities of a specific layer over a short duration.
/// </summary>
public class PoisonSmoke : MonoBehaviour
{
    [SerializeField] private LayerMask whatCanBeDamaged;
    [SerializeField] private float damageInterval = 0.5f;
    [SerializeField] private float damagePerTick = 3f;

    private Dictionary<Collider2D, float> lastDamageTime = new Dictionary<Collider2D, float>();

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!PhysicsUtils.IsGameObjectInLayer(collision.gameObject, whatCanBeDamaged))
            return;

        if (!lastDamageTime.ContainsKey(collision))
            lastDamageTime[collision] = -damageInterval;

        if (Time.time - lastDamageTime[collision] >= damageInterval)
        {
            lastDamageTime[collision] = Time.time;

            Debug.Log($"Deal {damagePerTick} Poison Smoke Damage to {collision.name}!");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (lastDamageTime.ContainsKey(collision))
            lastDamageTime.Remove(collision);
    }
}
