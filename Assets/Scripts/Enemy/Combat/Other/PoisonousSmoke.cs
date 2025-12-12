using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Deals continuous damage to all entities of a specific layer over a short duration.
/// </summary>
public class PoisonousSmoke : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [SerializeField] private float lifeSpan = 5;
    [SerializeField] private int damagePerTick = 2;
    [SerializeField] private float damageInterval = 1f;
    [SerializeField] private float totalPoisonTime = 3f;

    private PlayerPoisonHandler poisonHandler;

    private void Awake()
    {
        StartCoroutine(WaitThenDisappear());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        poisonHandler = collision.GetComponent<PlayerPoisonHandler>();
        if (poisonHandler == null) return;

        poisonHandler.ApplyPoisonEffect(damagePerTick, damageInterval, totalPoisonTime);
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (poisonHandler == null) return;

        poisonHandler.ApplyPoisonEffect(damagePerTick, damageInterval, totalPoisonTime);
    }

    private IEnumerator WaitThenDisappear()
    {
        yield return new WaitForSeconds(lifeSpan);

        animator.SetBool("disappear", true);

        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }
}
