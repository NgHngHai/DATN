using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float projectileLifeSpan = 2;
    [SerializeField] private bool dontDestroyOnHit;

    [Header("Hit Effect")]
    [SerializeField] private GameObject hitEffect;
    
    protected Rigidbody2D rb;
    protected bool alreadyHit;
    private HurtBox hurtBox;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hurtBox = GetComponent<HurtBox>();
        Destroy(gameObject, projectileLifeSpan);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Utility.IsGameObjectInLayer(collision.gameObject, hurtBox.targetLayers))
            return;

        alreadyHit = true;
        Vector2 hitPoint = collision.ClosestPoint(transform.position);
        CreateHitEffect(hitPoint);

        if (!dontDestroyOnHit)
            Destroy(gameObject);
    }

    protected void CreateHitEffect(Vector2 hitPoint)
    {
        if (hitEffect != null)
        {
            GameObject newHitEffect = Instantiate(hitEffect, hitPoint, transform.rotation);
        }
    }

    public void SetVelocity(Vector2 velocity)
    {
        rb.linearVelocity = velocity;
    }
}
