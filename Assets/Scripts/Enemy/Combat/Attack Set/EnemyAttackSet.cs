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

    #region Core Functions
    public void TryCurrentAttack()
    {
        currentAttack?.TryAttack();
    }

    public bool IsCurrentAttackReady()
    {
        return currentAttack.IsReadyToAttack();
    }

    public bool IsTargetInCurrentAttackArea(Transform target)
    {
        return currentAttack.IsTargetInAttackArea(target);
    }
    #endregion

    #region Change Attack Type
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
    #endregion

    public EnemyAttackBehavior CurrentAttack => currentAttack;
}
