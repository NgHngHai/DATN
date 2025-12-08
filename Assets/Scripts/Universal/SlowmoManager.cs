using UnityEngine;
using System.Collections;

/// Centralized, resilient slow-motion manager that survives GameObject disable/destroy
/// and guarantees restoration. Uses realtime scheduling and reference counting.

public sealed class SlowMoManager : MonoBehaviour
{
    private static SlowMoManager _instance;
    public static SlowMoManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Try find an existing instance in the scene first
                _instance = FindFirstObjectByType<SlowMoManager>();
                if (_instance == null)
                {
                    var go = new GameObject("SlowmoManager");
                    _instance = go.AddComponent<SlowMoManager>();
                }
                _instance.InitializeIfNeeded();
            }
            return _instance;
        }
    }

    [SerializeField] private float heavyHitTimeScale = 0.5f;
    [SerializeField] private float heavyHitDuration = 0.3f;
    private bool _initialized;
    private float _baseFixedDeltaTime;
    private int _activeRequests;
    private float _currentTargetScale = 1f;
    private float _endRealtime;
    private Coroutine _routine;

    private void Awake()
    {
        // Standard singleton guard
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        InitializeIfNeeded();
    }

    private void InitializeIfNeeded()
    {
        if (_initialized) return;
        DontDestroyOnLoad(gameObject);
        _baseFixedDeltaTime = Time.fixedDeltaTime;
        _initialized = true;
    }

    /// Request slow-mo at the given scale for at least duration seconds (realtime).
    /// If called again while active, extends end time and applies the lowest scale requested.
    public void Request()
    {
        // Clamp inputs
        heavyHitTimeScale = Mathf.Clamp(heavyHitTimeScale, 0.01f, 1f);
        heavyHitDuration = Mathf.Max(0f, heavyHitDuration);

        // Extend end time and use the lowest requested scale
        _endRealtime = Mathf.Max(_endRealtime, Time.realtimeSinceStartup + heavyHitDuration);
        float newScale = Mathf.Min(_currentTargetScale, heavyHitTimeScale);

        // Apply scale if lowered
        if (Time.timeScale > newScale)
        {
            _currentTargetScale = newScale;
            Time.timeScale = _currentTargetScale;
            Time.fixedDeltaTime = _baseFixedDeltaTime * _currentTargetScale;
        }
        else
        {
            _currentTargetScale = Mathf.Min(_currentTargetScale, heavyHitTimeScale);
        }

        if (_routine == null)
            _routine = StartCoroutine(SlowMoRoutine());
    }

    /// Force restore immediately. Safe to call multiple times.
    public void RestoreNow()
    {
        _endRealtime = 0f;
        _currentTargetScale = 1f;

        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }

        Time.timeScale = 1f;
        Time.fixedDeltaTime = _baseFixedDeltaTime;
    }

    private IEnumerator SlowMoRoutine()
    {
        while (true)
        {
            // Wait until the latest requested end time
            while (Time.realtimeSinceStartup < _endRealtime)
                yield return null;

            _activeRequests = 0;
            _currentTargetScale = 1f;
            Time.timeScale = 1f;
            Time.fixedDeltaTime = _baseFixedDeltaTime;

            _routine = null;
            yield break;
        }
    }
}