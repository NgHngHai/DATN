using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerController))]
public class PlayerMoneyManager : MonoBehaviour, ISaveable
{
    [Serializable]
    public class MoneyChangedEvent : UnityEvent<int, int> { } // current money, amount changed

    [Header("Money Settings")]
    [Tooltip("Starting money if no saved data is restored.")]
    [SerializeField] private int startingMoney = 0;

    [SerializeField] private int currentMoney = 0;

    [Header("Events")]
    [Tooltip("Invoked whenever the player's money changes. Parameter: current money.")]
    public MoneyChangedEvent OnMoneyChanged = new();

    public int CurrentMoney
    {
        get
        {
            return currentMoney;
        }
        set
        {
            currentMoney = Mathf.Max(0, value);
            OnMoneyChanged?.Invoke(currentMoney, 0);
        }
    }

    private bool _restoredFromSave = false;

    private void Awake()
    {
        if (!_restoredFromSave)
        {
            currentMoney = Mathf.Max(0, startingMoney);
            OnMoneyChanged?.Invoke(currentMoney, 0);
        }
    }

    // Increase money immediately
    public void AddMoney(int amount)
    {
        if (amount <= 0) return;

        // Clamp to avoid overflow
        long sum = (long)currentMoney + amount;
        currentMoney = (int)Mathf.Clamp(sum, 0, int.MaxValue);

        OnMoneyChanged?.Invoke(currentMoney, amount);
    }

    // Increase money after a delay (in seconds). If delay <= 0, adds immediately.
    public void AddMoneyDelayed(int amount, float delaySeconds)
    {
        if (amount <= 0) return;

        if (delaySeconds <= 0f)
        {
            AddMoney(amount);
            return;
        }

        StartCoroutine(DoAddMoneyDelayed(amount, delaySeconds));
    }

    private IEnumerator DoAddMoneyDelayed(int amount, float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        AddMoney(amount);
    }

    // Spend money if enough available; returns true if spent, false if insufficient funds
    public bool TrySpend(int amount)
    {
        if (amount <= 0) return true; // nothing to spend
        if (currentMoney < amount) return false;

        currentMoney -= amount;
        OnMoneyChanged?.Invoke(currentMoney, amount);
        return true;
    }

    // For systems that must deduct regardless of balance (use carefully)
    public void ForceSpend(int amount)
    {
        if (amount <= 0) return;

        currentMoney = Mathf.Max(0, currentMoney - amount);
        OnMoneyChanged?.Invoke(currentMoney, amount);
    }

    // ISaveable implementation (so it participates in your room/scene save pass)
    public string GetUniqueID() => "Player-Money";

    public object CaptureState()
    {
        return currentMoney;
    }

    public void RestoreState(object state)
    {
        if (state is int saved)
        {
            _restoredFromSave = true;
            currentMoney = Mathf.Max(0, saved);
            OnMoneyChanged?.Invoke(currentMoney, 0);
        }
    }
}
