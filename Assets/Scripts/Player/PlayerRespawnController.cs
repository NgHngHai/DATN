using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
#endif

/// Packages death -> overlay -> delayed respawn at last checkpoint -> revive.
[DisallowMultipleComponent]
public class PlayerRespawnController : MonoBehaviour
{
    [Header("Respawn Settings")]
    [Tooltip("Delay before respawning after death.")]

    private RoomManager _roomManager;
    private PlayerController _player;
    private EffectEvents _effectEvents;
    private PlayerSaveables _save;
    private Health _health;
    private GameObject overlayGameObject;

    private bool _overlayFinished = false;

    private void Awake()
    {
        _roomManager = FindAnyObjectByType<RoomManager>();
        _player = GetComponent<PlayerController>();
        _effectEvents = GetComponent<EffectEvents>();
        _save = GetComponent<PlayerSaveables>();
        _health = GetComponent<Health>();

        // Find overlay by tag
        overlayGameObject = GameObject.FindWithTag("Respawn Overlay");
    }

    //private void OnEnable()
    //{
    //    if (_health != null)
    //        _health.OnDeath.AddListener(HandleDeath);
    //}

    //private void OnDisable()
    //{
    //    if (_health != null)
    //        _health.OnDeath.RemoveListener(HandleDeath);
    //}

    // Animation event, called at the end of the death animation
    public void StartRespawn()
    {
        Debug.Log("PlayerRespawnController: StartRespawn called.");
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        if (_player != null)
        {
            _player.movementLocked = true;
        }

        // Enable Respawn Overlay
        _overlayFinished = false;
        StartCoroutine(PlayRespawnOverlayVideoOrSkip(() => _overlayFinished = true));

        if (_roomManager == null || _save == null) yield break;

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
            spawnPos = new Vector3(0, 0, 0);
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

        while (!_overlayFinished)
            yield return null;

        _player.PlayReviveState();
        _player.movementLocked = false;
        _effectEvents.OnRespawn?.Invoke();
    }

    private IEnumerator PlayRespawnOverlayVideoOrSkip(System.Action onFinished)
    {


        if (overlayGameObject == null)
        {
            onFinished?.Invoke();
            yield break;
        }

        // Ensure overlay is visible
        overlayGameObject.SetActive(true);

        // Attempt to get a VideoPlayer
        var videoPlayer = overlayGameObject.GetComponentInChildren<VideoPlayer>();

        // If no VideoPlayer present, just show overlay briefly then continue
        if (videoPlayer == null)
        {
            yield return null;
            overlayGameObject.SetActive(false);
            onFinished?.Invoke();
            yield break;
        }

        // Prepare and play video
        bool finished = false;
        VideoPlayer.EventHandler onLoopPoint = (VideoPlayer _) => { finished = true; };
        videoPlayer.loopPointReached += onLoopPoint;

        if (!videoPlayer.isPrepared)
        {
            videoPlayer.Prepare();
            while (!videoPlayer.isPrepared)
                yield return null;
        }

        videoPlayer.Play();

        // Wait until video completes OR any key is pressed (skip)
        while (!finished && videoPlayer.isPlaying)
        {
            if (AnyInputPressedThisFrame())
            {
                videoPlayer.Stop();
                break;
            }
            yield return null;
        }

        // Cleanup overlay
        videoPlayer.loopPointReached -= onLoopPoint;
        overlayGameObject.SetActive(false);

        onFinished?.Invoke();
    }

    private static bool AnyInputPressedThisFrame()
    {
#if ENABLE_INPUT_SYSTEM
        // Check all devices for any ButtonControl pressed this frame
        var devices = InputSystem.devices;
        for (int i = 0; i < devices.Count; i++)
        {
            var device = devices[i];
            var controls = device.allControls;
            for (int j = 0; j < controls.Count; j++)
            {
                var button = controls[j] as ButtonControl;
                if (button != null && button.wasPressedThisFrame)
                    return true;
            }
        }
        return false;
#elif ENABLE_LEGACY_INPUT_MANAGER
        return Input.anyKeyDown;
#else
        return false;
#endif
    }
}