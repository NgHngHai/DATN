using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Deals continuous damage to all entities of a specific layer over a short duration.
/// </summary>
public class PoisonousSmoke : MonoBehaviour
{
    [SerializeField] private SpriteRenderer poisonSpriteRenderer;
    [SerializeField] private int damagePerTick = 2;
    [SerializeField] private float damageInterval = 1f;
    [SerializeField] private float totalPoisonTime = 3f;
    [SerializeField] private float lifeSpan = 5;

    private float timer;

    private void Awake()
    {
        Destroy(gameObject, lifeSpan);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        float alpha = Mathf.Lerp(1, 0, timer / lifeSpan);

        Color fadeColor = poisonSpriteRenderer.color;
        fadeColor.a = alpha;

        poisonSpriteRenderer.color = fadeColor;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerPoisonHandler poisonHandler = collision.GetComponent<PlayerPoisonHandler>();
        if (poisonHandler == null) return;

        poisonHandler.ApplyPoisonEffect(damagePerTick, damageInterval, totalPoisonTime);
    }
}
