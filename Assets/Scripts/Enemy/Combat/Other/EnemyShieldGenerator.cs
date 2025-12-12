using System.Collections.Generic;
using UnityEngine;

public class EnemyShieldGenerator : MonoBehaviour
{
    [SerializeField] private GameObject shieldPrefab;
    [SerializeField] private LayerMask enemyMask;

    private Dictionary<GameObject, EnemyShield> shieldedEnemiesBefore = new();
    private GameObject owner;

    private void Awake()
    {
        owner = transform.parent != null ? transform.parent.gameObject : null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject colObject = collision.gameObject;

        if (colObject != owner && PhysicsUtils.IsGameObjectInLayer(colObject, enemyMask))
        {
            if (!shieldedEnemiesBefore.TryGetValue(colObject, out EnemyShield shield))
            {
                shield = Instantiate(shieldPrefab, collision.transform)
                         .GetComponent<EnemyShield>();
                shield.transform.localPosition = Vector3.zero;
                shield.CanRegenerate = true;

                shieldedEnemiesBefore.Add(colObject, shield);
            }

            shield.TryActivate();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject colObject = collision.gameObject;

        if (colObject != owner && PhysicsUtils.IsGameObjectInLayer(collision.gameObject, enemyMask))
        {
            if (shieldedEnemiesBefore.TryGetValue(colObject, out EnemyShield shield))
            {
                shield.CanRegenerate = false;
            }
        }
    }
}
