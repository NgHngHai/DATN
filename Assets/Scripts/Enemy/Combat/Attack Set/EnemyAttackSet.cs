using UnityEngine;
using System;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class EnemyAttackSet : MonoBehaviour
{
    [Tooltip("Automatically find child AttackBehaviors if left blank.")]
    [SerializeField] private List<EnemyAttackBehavior> attackBehaviors = new();

    private Dictionary<Type, EnemyAttackBehavior> attackMap = new();
    private EnemyAttackBehavior currentAttack;

    private void Awake()
    {
        if (attackBehaviors == null || attackBehaviors.Count == 0)
        {
            attackBehaviors = new List<EnemyAttackBehavior>(GetComponentsInChildren<EnemyAttackBehavior>());
        }

        foreach (var attack in attackBehaviors)
        {
            if (attack != null && !attackMap.ContainsKey(attack.GetType()))
            {
                attackMap.Add(attack.GetType(), attack);
            }
        }

        if (attackBehaviors.Count > 0)
            currentAttack = attackBehaviors[0];
    }

    public void ChangeAttackType(int attackIndex)
    {
        if (attackIndex < 0 || attackIndex >= attackBehaviors.Count) return;
        currentAttack = attackBehaviors[attackIndex];
    }

    public bool ChangeAttackType<T>() where T : EnemyAttackBehavior
    {
        Type targetType = typeof(T);

        if (attackMap.TryGetValue(targetType, out EnemyAttackBehavior nextAttack))
        {
            currentAttack = nextAttack;
            return true;
        }

        Debug.LogError($"[EnemyAttackSet] Cannot find attack behavior of type {targetType.Name}.");
        return false;
    }

    public void IncreaseAttackSpeed(float percent, float effectTime)
    {
        foreach (var atk in attackBehaviors)
        {
            atk.IncreaseAttackSpeed(percent, effectTime);
        }
    }

    public EnemyAttackBehavior CurrentAttack => currentAttack;
}
