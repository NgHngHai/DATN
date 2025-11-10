using UnityEngine;

/// <summary>
/// Controls bullet behavior — movement, collision, and destruction.
/// Spawns a hit effect and destroys itself on impact or after a set time.
/// Supports separate obstacle and damage layers.
/// </summary>
public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject hitEffect;

    private int damage = 5;
    private LayerMask obstacleMask;
    private LayerMask damageMask;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, 4f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject target = collision.gameObject;

        bool hitObstacle = PhysicsUtils.IsGameObjectInLayer(target, obstacleMask);
        bool hitDamageable = PhysicsUtils.IsGameObjectInLayer(target, damageMask);

        if (hitObstacle || hitDamageable)
        {
            if (hitDamageable)
            {
                IDamageable damageable = target.GetComponent<IDamageable>();
                if (damageable != null)
                    damageable.TakeDamage(damage);
            }

            if (hitEffect != null)
                Instantiate(hitEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }

    public void Initialize(Vector2 velocity, int damage, LayerMask obstacleMask, LayerMask damageMask)
    {
        this.damage = damage;
        this.obstacleMask = obstacleMask;
        this.damageMask = damageMask;
        rb.linearVelocity = velocity;
    }
}
