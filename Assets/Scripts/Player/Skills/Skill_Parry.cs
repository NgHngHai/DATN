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
    [SerializeField] private ParticleSystem parryEffect;

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

        if (_col) _col.enabled = false;
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
        _counterGranted = false;

        _playerController.movementLocked = true;
        _playerController.animStateLocked = true;

        _playerController.animStateMachine.ChangeState(parryState);

        _playerHealth.isInvincible = true; // Invincible during parry

        if (_col) _col.enabled = true;

        // Wait for whatever clip is currently playing on layer 0, then end parry.
        StartCoroutine(EndParryAfterCurrentStateLength());
    }

    private IEnumerator EndParryAfterCurrentStateLength()
    {
        var animator = _playerController.animator;
        const int layer = 0;

        // Let the state apply (trigger/boolean change takes a frame)
        yield return null;

        // Read the active state's effective duration (accounts for Animator/state speed)
        var info = animator.GetCurrentAnimatorStateInfo(layer);
        float effectiveSpeed = Mathf.Max(0.0001f, animator.speed * info.speed);
        float duration = info.length / effectiveSpeed;

        yield return new WaitForSeconds(duration);

        EndParry();
        parryState.Exit(); // clear the "parry" bool
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
        if (_col) _col.enabled = false;

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
        if (_col != null && _col.enabled && other.GetComponent<HurtBox>())
        {
            // Rotating counter flash effect
            // Compute per-particle rotation to face the HurtBox (2D Z-axis), in radians
            if (parryEffect != null)
            {
                Vector3 sourcePos = transform.position;
                Vector3 targetPos = other.bounds.ClosestPoint(sourcePos);
                Vector3 dir = (targetPos - sourcePos).normalized;

                float angleZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg * -1; // radians

                // Emit one particle with explicit rotation (per-particle)
                var emitParams = new ParticleSystem.EmitParams
                {
                    rotation = angleZ
                };

                parryEffect.Emit(emitParams, 1);
            }

            if (counterAmount < maxCounterAmount && !_counterGranted)
            {
                counterAmount++;
                _counterGranted = true;
                parryEffect?.Play();
                OnCounterChanged?.Invoke(counterAmount, maxCounterAmount);
            }
        }
    }
}
