using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Health))]
public class MoneyOnDeath : MonoBehaviour
{
    [Header("Money Reward")]
    [Tooltip("How much money the player should receive when this object dies.")]
    public int moneyReward = 10;

    [Tooltip("Delay between this object's death and the actual money being added to the player.")]
    public float rewardDelay = 0.25f;

    private Health _health;
    private bool _awarded;

    private void Awake()
    {
        _health = GetComponent<Health>();
        if (_health != null)
            _health.OnDeath.AddListener(OnDied);
    }

    private void OnDestroy()
    {
        if (_health != null)
            _health.OnDeath.RemoveListener(OnDied);
    }

    private void OnDied()
    {
        if (_awarded) return; // ensure only once
        _awarded = true;

        var player = PlayerController.Instance;
        if (player == null)
        {
            Debug.LogWarning("[MoneyOnDeath] No PlayerController.Instance found to award money.");
            return;
        }

        var money = player.GetComponent<PlayerMoneyManager>();
        if (money == null)
        {
            Debug.LogWarning("[MoneyOnDeath] PlayerMoneyManager not found on Player to award money.");
            return;
        }

        money.AddMoneyDelayed(moneyReward, rewardDelay);
    }
}
