using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerPoisonHandler : MonoBehaviour
{
    [SerializeField] private Color poisonColor = Color.green;

    private int firstDamageContact = 3;
    private SpriteRenderer playerSR;
    private Health playerHealth;
    private Color baseColor;

    private bool isStillPoisoned;

    private void Awake()
    {
        playerHealth = GetComponent<Health>();
        playerSR = GetComponent<SpriteRenderer>();
        baseColor = playerSR.color;
    }

    public void ApplyPoisonEffect(int damagePerTick, float damageInterval, float totalPoisonTime)
    {
        if (isStillPoisoned || playerHealth == null || playerHealth.IsDead() || !playerHealth.CanBeDamaged())
            return;

        StartCoroutine(DoPoisonDamage(damagePerTick, damageInterval, totalPoisonTime));
    }

    private IEnumerator DoPoisonDamage(int damagePerTick, float damageInterval, float totalPoisonTime)
    {
        isStillPoisoned = true;
        playerSR.color = poisonColor;
        DealDamageToPlayer(firstDamageContact);

        float elapsed = 0f;
        while (elapsed < totalPoisonTime)
        {
            if (playerHealth == null || playerHealth.IsDead())
                break;

            DealDamageToPlayer(damagePerTick);

            yield return new WaitForSeconds(damageInterval);
            elapsed += damageInterval;
        }

        isStillPoisoned = false;
        playerSR.color = baseColor;
    }

    private void DealDamageToPlayer(int dmg)
    {
        playerHealth.TakeDamage(dmg, DamageType.Poison, Vector2.up, false);
    }
}