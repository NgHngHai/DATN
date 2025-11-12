using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private GameObject hitEffect;
    
    private HurtBox hurtBox;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hurtBox = GetComponent<HurtBox>();
        Destroy(gameObject, 4f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!PhysicsUtils.IsGameObjectInLayer(collision.gameObject, hurtBox.targetLayers)) return;

        if (hitEffect != null)
            Instantiate(hitEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    public void SetVelocity(Vector2 velocity)
    {
        rb.linearVelocity = velocity;
    }
}
