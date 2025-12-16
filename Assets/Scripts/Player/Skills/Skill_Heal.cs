using System.Collections;
using UnityEngine;

public class Skill_Heal : MonoBehaviour, ISkill
{
    // Heal params
    [Header("Heal Settings")]
    [SerializeField] private int _healAmount = 20;
    [SerializeField] private float _healDuration = 1f;
    [SerializeField] private ParticleSystem _healParticle;

    private Coroutine _healRoutine;
    private bool _cancelRequested;

    public AnimationState healState;

    // References
    [SerializeField] private GameObject player;
    private PlayerController _playerController;
    private Health _healthComponent;

    private void Awake()
    {
        _healthComponent = player.GetComponent<Health>();
        _playerController = player.GetComponent<PlayerController>();

        healState = new AnimationState(_playerController, "heal", true);
    }

    private void OnEnable()
    {
        _healthComponent.OnDamagedWithReaction.AddListener(CancelHealing);
    }

    private void OnDisable()
    {
        _healthComponent.OnDamagedWithReaction.RemoveListener(CancelHealing);

        if (_healRoutine != null)
        {
            _cancelRequested = true;
            _playerController.movementLocked = false;
            _playerController.skillLocked = false;
            if (_healParticle != null)
                _healParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            _healRoutine = null;
        }
    }

    // Heals the Health.cs component attached to gameObject
    public void Activate()
    {
        if (_healRoutine != null) return;

        _cancelRequested = false;
        _healRoutine = StartCoroutine(HealRoutine());
    }

    private IEnumerator HealRoutine()
    {
        _playerController.movementLocked = true;
        _playerController.animStateLocked = true;
        _playerController.skillLocked = true;
        _playerController.animStateMachine.ChangeState(healState);

        float elapsed = 0f;
        while (elapsed < _healDuration)
        {
            if (_playerController.animStateMachine.currentState != healState || _cancelRequested)
            {
                _cancelRequested = false;
                _playerController.animStateLocked = false;
                _playerController.skillLocked = false;
                _healParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                _healRoutine = null;
                yield break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (_playerController.animStateMachine.currentState == healState && !_cancelRequested)
        {
            FinishHealing();
        }

        _healRoutine = null;
    }

    private void CancelHealing(int damage, Vector2 direction, bool shouldTriggerReaction)
    {
        if (damage <= 0 || _healRoutine == null) return;

        _cancelRequested = true;

        if (shouldTriggerReaction)
        {
            _playerController.animStateLocked = false;
            _playerController.skillLocked = false;
            _healParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    private void FinishHealing()
    {
        _healParticle.Play();

        _healthComponent.Heal(_healAmount);
        _playerController.movementLocked = false;
        _playerController.animStateLocked = false;
        _playerController.skillLocked = false;
    }
}
