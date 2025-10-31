using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerPoisonHandler : MonoBehaviour
{
    private Health playerHealth;
    private Coroutine poisonCoroutine;

    private void Awake()
    {
        playerHealth = GetComponent<Health>();
    }

    public void ApplyPoisonEffect(int damagePerTick, float damageInterval, float totalPoisonTime)
    {
        if (playerHealth == null || playerHealth.IsDead())
            return;

        if (poisonCoroutine != null)
            StopCoroutine(poisonCoroutine);

        poisonCoroutine = StartCoroutine(DoPoisonDamage(damagePerTick, damageInterval, totalPoisonTime));
    }

    private IEnumerator DoPoisonDamage(int damagePerTick, float damageInterval, float totalPoisonTime)
    {
        float elapsed = 0f;
        while (elapsed < totalPoisonTime)
        {
            if (playerHealth == null || playerHealth.IsDead() || !playerHealth.CanBeDamaged())
                break;

            playerHealth.TakeDamage(damagePerTick, false);
            yield return new WaitForSeconds(damageInterval);
            elapsed += damageInterval;
        }

        poisonCoroutine = null;
    }

}