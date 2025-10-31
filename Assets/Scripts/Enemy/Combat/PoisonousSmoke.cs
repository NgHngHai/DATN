using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Deals continuous damage to all entities of a specific layer over a short duration.
/// </summary>
public class PoisonousSmoke : MonoBehaviour
{
    [SerializeField] private LayerMask whatCanBeDamaged;
    [SerializeField] private int damageFirstContact = 3;
    [SerializeField] private int damagePerTick = 2;
    [SerializeField] private float damageInterval = 1f;
    [SerializeField] private float totalPoisonTime = 3f;

    private List<IDamageable> alreadyDamagedList = new List<IDamageable>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable == null || alreadyDamagedList.Contains(damageable)) return;

        alreadyDamagedList.Add(damageable);
        damageable.TakeDamage(damageFirstContact);

        PlayerPoisonHandler poisonHandler = collision.GetComponent<PlayerPoisonHandler>();
        if (poisonHandler == null) return;

        poisonHandler.ApplyPoisonEffect(damagePerTick, damageInterval, totalPoisonTime);
    }

}
