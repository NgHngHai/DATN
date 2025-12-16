using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

/// Packages death -> overlay -> delayed respawn at last checkpoint -> revive.
[DisallowMultipleComponent]
public class PlayerRespawnController : MonoBehaviour
{
    [Header("Respawn Settings")]
    [Tooltip("Delay before respawning after death.")]
    [SerializeField] private float respawnDelaySeconds = 1.5f;

    [Header("Overlay/UI")]
    [Tooltip("Invoked when death occurs so UI can show overlay.")]
    public UnityEvent OnRespawnOverlayRequested;

    private RoomManager _roomManager;
    private PlayerController _player;
    private EffectEvents _effectEvents;
    private PlayerSaveables _save;
    private Health _health;

    private void Awake()
    {
        _roomManager = FindAnyObjectByType<RoomManager>();
        _player = GetComponent<PlayerController>();
        _effectEvents = GetComponent<EffectEvents>();
        _save = GetComponent<PlayerSaveables>();
        _health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        if (_health != null)
            _health.OnDeath.AddListener(HandleDeath);
    }

    private void OnDisable()
    {
        if (_health != null)
            _health.OnDeath.RemoveListener(HandleDeath);
    }

    private void HandleDeath()
    {
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        // Show overlay
        OnRespawnOverlayRequested?.Invoke();

        // Delay
        yield return new WaitForSeconds(respawnDelaySeconds);

        if (_roomManager == null || _player == null || _save == null) yield break;

        bool hasCheckpoit = !string.IsNullOrEmpty(_save.lastCheckpointRoomName);
        string targetRoom = hasCheckpoit ? _save.lastCheckpointRoomName : "Room1";

        if (!string.IsNullOrEmpty(targetRoom) && SceneManager.GetActiveScene().name != targetRoom)
        {
            _roomManager.LoadRoomWithTransitionDirection(targetRoom, EnterDirection.Top);

            // Wait until the requested room becomes active
            while (SceneManager.GetActiveScene().name != targetRoom)
                yield return null;
        }

        // Resolve the room’s single checkpoint position (fallback to FirstSpawnPosition)
        var roomData = FindAnyObjectByType<RoomData>();
        Vector2 spawnPos;

        if (hasCheckpoit)
        {
            spawnPos = roomData.GetRoomCheckpointPosition();
        } 
        else
        {
            spawnPos = roomData.FirstSpawnPosition;
        }

            // Move player and restore
            _player.transform.position = spawnPos;

        if (_player.TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.gravityScale = _player.originalGravityMultiplier;
        }

        _health.RestoreToFull();
        _player.PlayReviveState();
        //_player.isAlive = true;
        _player.movementLocked = false;
        _effectEvents.OnRespawn?.Invoke();
    }
}