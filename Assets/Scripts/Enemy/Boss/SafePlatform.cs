using System.Collections.Generic;
using UnityEngine;

public class SafePlatform : MonoBehaviour
{
    [SerializeField] private LayerMask safeLayer;

    private List<Health> healthList = new List<Health> ();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(PhysicsUtils.IsGameObjectInLayer(collision.gameObject, safeLayer))
        {
            Health health = collision.GetComponent<Health>();
            if (health != null)
            {
                health.isInvincible = true;
                healthList.Add(health);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (PhysicsUtils.IsGameObjectInLayer(collision.gameObject, safeLayer))
        {
            Health health = collision.GetComponent<Health>();
            if (health != null)
            {
                health.isInvincible = false;
                healthList.Remove(health);
            }
        }
    }

    private void OnDisable()
    {
        foreach (var h in healthList)
        {
            if (h != null)
                h.isInvincible = false;
        }
        healthList.Clear();
    }
}
