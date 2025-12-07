using UnityEngine;
using UnityEngine.UI;

public class OscillateAnim : MonoBehaviour
{
    [Header("Oscillation Settings")]
    [Tooltip("Movement distance in pixels from the starting position.")]
    [SerializeField] private float amplitude = 8f;

    [Tooltip("Oscillation speed (cycles per second).")]
    [SerializeField] private float frequency = 1.2f;

    [Tooltip("Offset the phase if you want multiple arrows out of sync.")]
    [SerializeField] private float phaseOffset = 0f;

    [Header("Options")]
    [Tooltip("Freeze when not visible to save work.")]
    [SerializeField] private bool pauseWhenDisabled = true;

    private RectTransform _rect;
    private Vector2 _startAnchoredPos;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _startAnchoredPos = _rect.anchoredPosition;
    }

    private void OnEnable()
    {
        // Reset position on enable to avoid drift when toggling.
        _rect.anchoredPosition = _startAnchoredPos;
    }

    private void Update()
    {
        if (pauseWhenDisabled && !gameObject.activeInHierarchy)
            return;

        // Vertical bob using sine wave: y = A * sin(2πft + phase)
        float t = Time.unscaledTime; // UI often should ignore gameplay timescale; switch to Time.time if desired.
        float offsetY = amplitude * Mathf.Sin((Mathf.PI * 2f * frequency * t) + phaseOffset);

        // Apply only vertical oscillation, preserve original X.
        _rect.anchoredPosition = new Vector2(_startAnchoredPos.x, _startAnchoredPos.y + offsetY);
    }

    /// <summary>
    /// Allows runtime updates to oscillation parameters.
    /// </summary>
    public void Configure(float newAmplitude, float newFrequency, float newPhase = 0f)
    {
        amplitude = newAmplitude;
        frequency = newFrequency;
        phaseOffset = newPhase;
    }
}
