using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerPoisonHandler : MonoBehaviour
{
    [SerializeField] private Color poisonColor = Color.green;

    private SpriteRenderer playerSR;
    private Health playerHealth;
    private Coroutine poisonCoroutine;
    private Color baseColor;

    private void Awake()
    {
        playerHealth = GetComponent<Health>();
        playerSR = GetComponent<SpriteRenderer>();
        baseColor = playerSR.color;
    }

    public void ApplyPoisonEffect(int damagePerTick, float damageInterval, float totalPoisonTime)
    {
        if (playerHealth == null || playerHealth.IsDead() || !playerHealth.CanBeDamaged())
            return;

        if (poisonCoroutine != null)
            StopCoroutine(poisonCoroutine);

        poisonCoroutine = StartCoroutine(DoPoisonDamage(damagePerTick, damageInterval, totalPoisonTime));
    }

    private IEnumerator DoPoisonDamage(int damagePerTick, float damageInterval, float totalPoisonTime)
    {
        float elapsed = 0f;
        playerSR.color = poisonColor;

        while (elapsed < totalPoisonTime)
        {
            if (playerHealth == null || playerHealth.IsDead())
                break;

            playerHealth.TakeDamage(damagePerTick, DamageType.Normal, Vector2.up, false);
            yield return new WaitForSeconds(damageInterval);
            elapsed += damageInterval;
        }

        playerSR.color = baseColor;
        poisonCoroutine = null;
    }
}