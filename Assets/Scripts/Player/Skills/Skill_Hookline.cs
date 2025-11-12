using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Hookline : MonoBehaviour, ISkill
{
    // Hook point detection
    [Header("Detection")]
    [SerializeField] private LayerMask hookableLayers;
    [Tooltip("Center of detection area relative to player")]
    [SerializeField] private Vector2 detectionCenter;

    // Pulling params
    [Header("Pull params")]
    [SerializeField] private float maxSpeed = 35f;      // speed along rope
    [SerializeField] private float acceleration = 120f; // accel toward maxSpeed
    [SerializeField] private float arrivalDistance = 1f; // auto-cancel threshold at point
    [SerializeField] private float dampening = 0.6f;      // velocity damping factor on arrival

    [Header("References")]
    // Visuals
    [SerializeField] private LineRenderer rope;
    // Player reference
    public GameObject player;
    private PlayerController _playerController;

    private readonly List<Transform> _candidates = new();
    private Rigidbody2D _rb;
    private Coroutine _pullRoutine;
    private Vector2 _anchorWorld;
    private float _savedGravityScale;

    private void Awake()
    {
        _rb = player.GetComponent<Rigidbody2D>();
        _playerController = player.GetComponent<PlayerController>();
    }

    // Starts pull if not already pulling
    public void Activate()
    {
        if (_pullRoutine != null) return;

        var target = FindBestCandidate();
        if (target == null) return;

        StartPull(target.position);
    }

    private void StartPull(Vector2 anchorWorld)
    {
        //_currentTarget = target;
        _anchorWorld = anchorWorld;

        // Prevent player control during pull
        _playerController.movementLocked = true;

        if (rope != null)
        {
            rope.enabled = true;
            rope.positionCount = 2;
            rope.useWorldSpace = true;
        }

        _pullRoutine = StartCoroutine(PullRoutine());
    }

    private IEnumerator PullRoutine()
    {
        var wait = new WaitForFixedUpdate ();

        _savedGravityScale = _rb.gravityScale;

        float safetyTimer = 0f;
        const float safetyTimeout = 6f;

        while (true)
        {
            safetyTimer += Time.fixedDeltaTime;
            if (safetyTimer > safetyTimeout) break; // fail-safe

            Vector2 pos = _rb.position;
            Vector2 toAnchor = _anchorWorld - pos;
            float dist = toAnchor.magnitude;

            if (dist <= arrivalDistance) break; // break on arrival

            Vector2 dir = toAnchor / Mathf.Max(0.0001f, dist);

            Vector2 vel = _rb.linearVelocity;
            float along = Vector2.Dot(vel, dir);
            Vector2 perp = vel - dir * along;

            // Accelerate along rope toward maxSpeed
            float newAlong = Mathf.MoveTowards(along, maxSpeed, acceleration * Time.fixedDeltaTime);

            _rb.linearVelocity = dir * newAlong + perp * dampening;

            _rb.gravityScale = 0f;

            if (rope != null)
            {
                rope.SetPosition(0, pos);
                rope.SetPosition(1, _anchorWorld);
            }

            yield return wait;
        }

        // Restore gravity & controls, keep current velocity so player overshoots
        _rb.gravityScale = _savedGravityScale;

        _playerController.externalVelocityX = _rb.linearVelocity.x;
        _playerController.movementLocked = false;

        if (rope != null) rope.enabled = false;

        _pullRoutine = null;
    }

    // Detection: maintain set of hookable targets inside player's trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsHookable(other)) return;
        _candidates.Add(GetTargetRoot(other));
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var root = GetTargetRoot(other);
        if (root != null) _candidates.Remove(root);
    }

    private Transform FindBestCandidate()
    {
        Transform best = null;
        float bestDist = float.MaxValue;

        foreach (var t in _candidates)
        {
            if (t == null) continue;
            float d = Vector2.Distance(_rb.position + detectionCenter, t.position);
            if (d < bestDist)
            {
                best = t;
                bestDist = d;
            }
        }

        return best;
    }

    private bool IsHookable(Collider2D col)
    {
        return (((1 << col.gameObject.layer) & hookableLayers) != 0);
    }

    private Transform GetTargetRoot(Collider2D col)
    {
        if (col.attachedRigidbody != null) return col.attachedRigidbody.transform;
        return col.transform;
    }
}
