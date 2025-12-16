using TMPro;
using UnityEngine;

public class TutorialPromp : MonoBehaviour
{
    [Header("Detection")]
    [Tooltip("Layers considered as player for proximity detection.")]
    [SerializeField] private LayerMask playerLayerMask; // pick from LayerMask in inspector

    [Header("Prompt")]
    [Tooltip("Text to show when the player is in range.")]
    [SerializeField] private string inRangePromptVN = "Nhấn []";
    [SerializeField] private string inRangePromptEN = "Press [Interact]";
    [Tooltip("Offset (world units) applied to the prompt position relative to this object.")]
    [SerializeField] private Vector2 promptOffset = new Vector2(0f, 1.75f);

    // References
    [Header("References (Auto)")]
    [SerializeField] private Canvas _sharedCanvas;
    [SerializeField] private TMP_Text _sharedText;

    private bool _playerInRange;

    private void Awake()
    {
        // Ensure proximity collider is a trigger
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsInPlayerMask(other.gameObject.layer))
            return;

        _playerInRange = true;

        ShowSharedPromptAtThis();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsInPlayerMask(other.gameObject.layer))
            return;

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
}
