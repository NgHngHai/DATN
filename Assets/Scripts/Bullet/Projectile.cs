using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float projectileLifeSpan = 2;
    [SerializeField] private bool dontDestroyOnHit;

    [Header("Hit Effect")]
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private float hitEffectLifeSpan = 0.5f;
    
    private HurtBox hurtBox;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hurtBox = GetComponent<HurtBox>();
        Destroy(gameObject, projectileLifeSpan);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!PhysicsUtils.IsGameObjectInLayer(collision.gameObject, hurtBox.targetLayers))
            return;

        Vector2 hitPoint = collision.ClosestPoint(transform.position);

        if (hitEffect != null)
        {
            GameObject newHitEffect = Instantiate(hitEffect, hitPoint, transform.rotation);
            Destroy(newHitEffect, hitEffectLifeSpan);
        }

        if (!dontDestroyOnHit)
            Destroy(gameObject);
    }


    public void SetVelocity(Vector2 velocity)
    {
        rb.linearVelocity = velocity;
    }
}
