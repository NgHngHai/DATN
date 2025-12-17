using System.Collections;
using UnityEngine;

public class BossHandBreakAnimation : MonoBehaviour
{
    [SerializeField] private AudioSource explosionSource;
    [SerializeField] private ParticleSystem brokenExplosionPS;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D col;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private float fadeDelay = 3f;
    [SerializeField] private float fadeAndDestroyDuration = 2f;

    [Header("Break Force")]
    [SerializeField] private float pushForce = 40;
    [SerializeField] private float minForceAngle = 0f;
    [SerializeField] private float maxForceAngle = 90f;

    public void StartAnimation()
    {
        brokenExplosionPS.Play();
        transform.SetParent(null);
        explosionSource.Play();

        rb.bodyType = RigidbodyType2D.Dynamic;
        Vector2 breakDir = GetRandomDirection(minForceAngle, maxForceAngle);
        rb.angularVelocity = Random.Range(-400f, 400f);
        rb.AddForce(breakDir * pushForce, ForceMode2D.Impulse);

        col.enabled = true;

        StartCoroutine(WaitThenFadeAndDestroy());
    }

    private IEnumerator WaitThenFadeAndDestroy()
    {
        yield return new WaitForSeconds(fadeDelay);

        float elapsedTime = 0f;
        Color startColor = sr.color; 

        while (elapsedTime < fadeAndDestroyDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startColor.a, 0f, elapsedTime / fadeAndDestroyDuration);
            sr.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);

            yield return null;
        }

        sr.color = new Color(startColor.r, startColor.g, startColor.b, 0f);

        Destroy(gameObject);
    }

    private Vector2 GetRandomDirection(float minAngle, float maxAngle)
    {
        float angle = Random.Range(minAngle, maxAngle);
        float rad = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
    }

}
