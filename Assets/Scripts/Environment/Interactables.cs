using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

/// <summary>
/// Base interactable component.
/// - Detects player proximity via trigger.
/// - Shows a world-space canvas with prompt when player is in range.
///
/// Usage:
/// - Add this to an object with a Collider set to "Is Trigger".
/// - Assign a world-space Canvas (as child of the object) and a Text to display the prompt.
/// - Extend this class and override OnInteract to implement behavior (heal, open chest, etc.).
/// </summary>

[RequireComponent(typeof(Collider2D))]
public class Interactables : SaveableObject
{
    [Header("Detection")]
    [Tooltip("Layers considered as player for proximity detection.")]
    [SerializeField] protected LayerMask playerLayerMask; // pick from LayerMask in inspector

    [Header("Prompt")]
    [Tooltip("Text to show when the player is in range.")]
    [SerializeField] private string inRangePromptVN = "Nhấn []";
    [SerializeField] private string inRangePromptEN = "Press [Interact]";
    [Tooltip("Offset (world units) applied to the prompt position relative to this object.")]
    [SerializeField] private Vector2 promptOffset = new Vector2(0f, 1.75f);

    [Header("Interaction")]
    [Tooltip("Optional cooldown after interaction in seconds.")]
    [SerializeField] private float interactCooldown = 0f;

    [Header("Save options")]
    [Tooltip("Whether to load in after interacted.")]
    [SerializeField] protected bool isOneTimeUse = false;
    [Tooltip("Whether the player has interacted with this object.")]
    [SerializeField] protected bool playerInteracted = false;
    private bool spawnOnLoad = true;

    private bool _playerInRange;
    private float _cooldownUntil;

    // References
    [Header("References (Auto)")]
    [SerializeField] private Canvas _sharedCanvas;
    [SerializeField] private TMP_Text _sharedText;

    protected override void Awake()
    {
        base.Awake();
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;

        // Cache shared canvas/text once
        CacheSharedPrompt();
        HideSharedPrompt(); // hidden by default
    }

    private void OnDisable()
    {
        // If this was showing the prompt, hide it when disabled
        if (_playerInRange)
        {
            HideSharedPrompt();
            _playerInRange = false;
        }
    }

    private void HandlePlayerInteractPressed()
    {
        if (!_playerInRange)
        { 
            return;
        }

        if (Time.time < _cooldownUntil)
            return;

        var player = GameObject.FindGameObjectWithTag("Player");
        OnInteract(player);

        if (interactCooldown > 0f)
            _cooldownUntil = Time.time + interactCooldown;
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsInPlayerMask(other.gameObject.layer))
            return;
        // Subscribe to player interact input on enter to avoid conflicts
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.OnInteractPressed += HandlePlayerInteractPressed;
        }


        _playerInRange = true;

        ShowSharedPromptAtThis();
    }

    protected void OnTriggerExit2D(Collider2D other)
    {
        if (!IsInPlayerMask(other.gameObject.layer))
            return;
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.OnInteractPressed -= HandlePlayerInteractPressed;
        }


        _playerInRange = false;

        HideSharedPrompt();
    }

    private bool IsInPlayerMask(int layer)
    {
        return (playerLayerMask.value & (1 << layer)) != 0;
    }

    private void CacheSharedPrompt()
    {
        if (_sharedCanvas != null && _sharedText != null)
            return;

        var canvasObj = GameObject.FindWithTag("World Space Canvas");
        if (canvasObj == null)
        {
            Debug.LogWarning("Interactables: No GameObject with tag 'World Space Canvas' found in scene.");
            return;
        }

        _sharedCanvas = canvasObj.GetComponent<Canvas>();
        if (_sharedCanvas == null)
        {
            Debug.LogWarning("Interactables: Tagged 'World Space Canvas' object does not have a Canvas component.");
            return;
        }

        if (_sharedCanvas.renderMode != RenderMode.WorldSpace)
        {
            Debug.LogWarning("Interactables: Shared Canvas should be set to World Space render mode.");
        }

        // Expect exactly one child with a Text component
        _sharedText = _sharedCanvas.GetComponentInChildren<TMP_Text>(true);
        if (_sharedText == null)
        {
            Debug.LogWarning("Interactables: No Text component found as a child of the shared Canvas.");
        }
    }

    private void ShowSharedPromptAtThis()
    {
        if (_sharedCanvas == null || _sharedText == null)
        {
            CacheSharedPrompt();
            if (_sharedCanvas == null || _sharedText == null) return;
        }

        // Set text content
        _sharedText.text = "[ " + inRangePromptEN + " ]";

        // Move the text to this object's position + offset
        var worldPos = transform.position + new Vector3(promptOffset.x, promptOffset.y, 0f);
        _sharedText.transform.position = worldPos;

        // Enable canvas (if it was hidden)
        if (!_sharedCanvas.enabled) _sharedCanvas.enabled = true;
        if (!_sharedText.gameObject.activeSelf) _sharedText.gameObject.SetActive(true);
    }

    private void HideSharedPrompt()
    {
        if (_sharedText != null) _sharedText.gameObject.SetActive(false);
        if (_sharedCanvas != null) _sharedCanvas.enabled = false;
    }

    protected virtual void OnInteract(GameObject player) { }
    //protected virtual void OnPlayerEnterRange(Collider2D playerCollider) { }
    //protected virtual void OnPlayerExitRange(Collider2D playerCollider) { }

    // Save system
    public override object CaptureState()
    {
        if (isOneTimeUse)
            spawnOnLoad = !playerInteracted;

        return new InteractableData
        {
            spawnOnLoad = this.spawnOnLoad,
            playerInteracted = playerInteracted
        };
    }

    public override void RestoreState(object state)
    {
        var saveData = Utility.ConvertState<InteractableData>(state);

        Debug.Log(saveData.spawnOnLoad);

        playerInteracted = saveData.playerInteracted;

        if (!saveData.spawnOnLoad)
        {
            gameObject.SetActive(false);
        }
    }

    [System.Serializable] private struct InteractableData
    {
        public bool spawnOnLoad;
        public bool playerInteracted;
    }

#if UNITY_EDITOR
        private void OnValidate()
        {
            var col = GetComponent<Collider>();
            if (col != null) col.isTrigger = true;
        }
#endif
}

