using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private LayerMask whatCanBeDamaged;
    [SerializeField] private float enableColTime = 0.1f;
    [SerializeField] private int damage;

    private void Awake()
    {
        Invoke(nameof(TurnColliderOff), enableColTime);
    }

    private List<IDamageable> alreadyDamagedEntities = new List<IDamageable>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!PhysicsUtils.IsGameObjectInLayer(collision.gameObject, whatCanBeDamaged)) return;

        IDamageable damageable = collision.GetComponent<IDamageable>();
        if(damageable != null && !alreadyDamagedEntities.Contains(damageable))
        {
            alreadyDamagedEntities.Add(damageable);
            damageable.TakeDamage(damage);
        }
    }

    private void TurnColliderOff()
    {
        GetComponent<Collider2D>().enabled = false;
    }
}
