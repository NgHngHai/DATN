using UnityEngine;

/// <summary>
/// Controls bullet behavior — movement, collision, and destruction.
/// Spawns a hit effect and destroys itself on impact or after a set time.
/// No damage logic yet.
/// </summary>
public class Bullet : MonoBehaviour
{
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private float damage;
    [SerializeField] private GameObject hitEffect;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, 4f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (PhysicsUtils.IsGameObjectInLayer(collision.gameObject, hitLayer))
        {
            if (hitEffect != null)
                Instantiate(hitEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }

    public void SetVelocity(Vector2 vel)
    {
        rb.linearVelocity = vel;
    }
}
