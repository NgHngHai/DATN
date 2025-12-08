using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
public class Interactables : MonoBehaviour
{
    [Header("Detection")]
    [Tooltip("Layers considered as player for proximity detection.")]
    [SerializeField] private LayerMask playerLayerMask; // pick from LayerMask in inspector

    [Header("Prompt")]
    [Tooltip("Text to show when the player is in range.")]
    [SerializeField] private string inRangePrompt = "Press [Interact]";
    [Tooltip("Offset (world units) applied to the prompt position relative to this object.")]
    [SerializeField] private Vector2 promptOffset = new Vector2(0f, 1.75f);

    [Header("Interaction")]
    [Tooltip("Optional cooldown after interaction in seconds.")]
    [SerializeField] private float interactCooldown = 0f;

    [Header("Outline")]
    [Tooltip("SpriteRenderer to outline. If null, will try to GetComponent<SpriteRenderer>().")]
    [SerializeField] private SpriteRenderer targetSprite;
    [Tooltip("Material that provides outline effect compatible with SpriteRenderer.")]
    [SerializeField] private Material outlineMaterial;
    [Tooltip("Outline width/intensity property name in the outline material (optional).")]
    [SerializeField] private string outlineWidthProperty = "_OutlineWidth";
    [Tooltip("Outline color property name in the outline material (optional).")]
    [SerializeField] private string outlineColorProperty = "_OutlineColor";
    [Tooltip("Outline width to apply when player is in range.")]
    [SerializeField] private float outlineWidth = 1.5f;
    [Tooltip("Outline color to apply when player is in range.")]
    [SerializeField] private Color outlineColor = Color.white;

    private bool _playerInRange;
    private float _cooldownUntil;
    // Cache original material to restore on exit
    private Material _originalMaterial;
    private bool _outlineApplied;

    // References
    [SerializeField] private Canvas _sharedCanvas;
    [SerializeField] private TMP_Text _sharedText;

    protected virtual void Awake()
    {
        // Ensure proximity collider is a trigger
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;

        targetSprite = GetComponent<SpriteRenderer>();
        _originalMaterial = targetSprite.sharedMaterial;

        // Cache shared canvas/text once
        CacheSharedPrompt();
        HideSharedPrompt(); // hidden by default
    }

    private void OnEnable()
    {
        // Subscribe to player interact input
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.OnInteractPressed += HandlePlayerInteractPressed;
        }
    }

    private void OnDisable()
    {
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.OnInteractPressed -= HandlePlayerInteractPressed;
        }

        // If this was showing the prompt, hide it when disabled
        if (_playerInRange)
        {
            HideSharedPrompt();
            RemoveOutline();
            _playerInRange = false;
        }
    }

    private void HandlePlayerInteractPressed()
    {
        if (!_playerInRange)
            return;

        if (Time.time < _cooldownUntil)
            return;

        var player = FindPlayerComponent();
        OnInteract(player);

        if (interactCooldown > 0f)
            _cooldownUntil = Time.time + interactCooldown;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsInPlayerMask(other.gameObject.layer))
            return;

        _playerInRange = true;

        ShowSharedPromptAtThis();
        ApplyOutline();

        OnPlayerEnterRange(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsInPlayerMask(other.gameObject.layer))
            return;

        _playerInRange = false;

        HideSharedPrompt();
        RemoveOutline();

        OnPlayerExitRange(other);
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
        _sharedText.text = "[ " + inRangePrompt + " ]";

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

    // Outline control
    private void ApplyOutline()
    {
        if (targetSprite == null || outlineMaterial == null)
            return;

        if (_outlineApplied)
            return;

        // Use instantiated material per renderer to avoid affecting shared material globally
        var instancedMat = new Material(outlineMaterial);

        // Set optional properties if present
        if (!string.IsNullOrEmpty(outlineWidthProperty) && instancedMat.HasProperty(outlineWidthProperty))
            instancedMat.SetFloat(outlineWidthProperty, outlineWidth);
        if (!string.IsNullOrEmpty(outlineColorProperty) && instancedMat.HasProperty(outlineColorProperty))
            instancedMat.SetColor(outlineColorProperty, outlineColor);

        targetSprite.material = instancedMat;
        _outlineApplied = true;
    }

    private void RemoveOutline()
    {
        if (targetSprite == null || !_outlineApplied)
            return;

        // Restore original material
        targetSprite.sharedMaterial = _originalMaterial;
        _outlineApplied = false;
    }

    // Attempts to find any component on an object that matches the player layer mask (override for a specific type).
    protected virtual Component FindPlayerComponent()
    {
        var allRoots = gameObject.scene.GetRootGameObjects();
        foreach (var root in allRoots)
        {
            var transforms = root.GetComponentsInChildren<Transform>(true);
            foreach (var t in transforms)
            {
                if (IsInPlayerMask(t.gameObject.layer))
                    return t.GetComponent<Component>();
            }
        }
        return null;
    }

    protected virtual void OnInteract(Component player) { }
    protected virtual void OnPlayerEnterRange(Collider2D playerCollider) { }
    protected virtual void OnPlayerExitRange(Collider2D playerCollider) { }


#if UNITY_EDITOR
        private void OnValidate()
        {
            var col = GetComponent<Collider>();
            if (col != null) col.isTrigger = true;
        }
#endif
}

