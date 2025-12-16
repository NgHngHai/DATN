using UnityEngine;
using System.Collections;

public class Skill_Parry: MonoBehaviour, ISkill
{
    public class CounterChangedEvent : UnityEngine.Events.UnityEvent<int, int> { } // (currentCounter, maxCounter)

    public CounterChangedEvent OnCounterChanged = new();

    // Parry params
    private bool _counterGranted = false;
    [SerializeField] private bool _missedParry = false;
    [Tooltip("The amount of time the player takes increased damage after missing parry.")]
    [SerializeField] private float increaseDamageDuration = 2f;
    [Tooltip("The amount of increased damage taken after missing parry.")]
    [SerializeField] private float increasedDamageTaken = 1.5f;

    // Counter params
    [Tooltip("The amount of time the player can use counter attack.")]
    [SerializeField] private int counterAmount = 0;
    [SerializeField] private int maxCounterAmount = 3;

    // Animation State
    public AnimationState parryState;

    // References
    private PlayerController _playerController;
    [SerializeField] private Collider2D _col;    
    private Health _playerHealth;

    public bool HasCounter => counterAmount > 0;

    private void Start()
    {
        _playerController = GetComponentInParent<PlayerController>();
        _playerHealth = GetComponentInParent<Health>();

        parryState = new AnimationState(_playerController, "parry", true);
    }

    public void Activate()
    {
        StartParry();
    }

    public bool TryUseCounter()
    {
        if (counterAmount <= 0) return false;

        counterAmount--;
        OnCounterChanged?.Invoke(counterAmount, maxCounterAmount);

        return true;
    }

    public void StartParry()
    {
        _missedParry = false;

        _playerController.movementLocked = true;
        _playerController.animStateLocked = true;

        _playerController.animStateMachine.ChangeState(parryState);

        _playerHealth.isInvincible = true; // Invincible during parry
    }

    public void EndParry()
    {
        if (!_counterGranted)
        {
            _missedParry = true;
        }

        _counterGranted = false;

        _playerController.movementLocked = false; // Unlock movement
        _playerController.animStateLocked = false; // Unlock anim state

        _playerHealth.isInvincible = false; // Disable invincibility

        if (_missedParry)
        {
            // Apply increased damage taken for a duration
            _playerHealth.StartCoroutine(IncreaseDamageTaken());
        }
    }

    private IEnumerator IncreaseDamageTaken()
    {
        _playerHealth.damageMultiplier = increasedDamageTaken;
        yield return new WaitForSeconds(increaseDamageDuration);
        _playerHealth.damageMultiplier = 1f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<HurtBox>())
        {
            if (counterAmount < maxCounterAmount && !_counterGranted)
            {
                counterAmount++;
                _counterGranted = true;
                OnCounterChanged?.Invoke(counterAmount, maxCounterAmount);

            }
        }
    }
}
