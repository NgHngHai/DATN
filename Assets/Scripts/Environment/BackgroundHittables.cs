using UnityEngine;
using System.Collections;

public class BackgroundHittables : MonoBehaviour, IDamageable
{
    [SerializeField] private float _maxRecoil = .2f;
    [SerializeField] private float _recoilSpeed = 5f;

    private Vector3 _initialPosition;
    private Coroutine _recoilCoroutine;

    void Start()
    {
        _initialPosition = transform.position;
    }

    public bool TakeDamage(int amount, DamageType type, Vector2 hitDir, bool shouldTriggerHitReaction = true)
    {
        if (_recoilCoroutine != null)
        {
            StopCoroutine(_recoilCoroutine);
        }

        _recoilCoroutine = StartCoroutine(RecoilBob());
        return true;
    }
    public bool CanBeDamaged() { return true; }
    public bool IsDead() { return false; }

    private IEnumerator RecoilBob()
    {
        Vector3 targetPos = _initialPosition - new Vector3(0, 1, 0) * _maxRecoil;

        // Move from current to down
        while ((transform.position - targetPos).sqrMagnitude > 0.000001f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                _recoilSpeed * Time.deltaTime
            );
            yield return null;
        }

        // Move back up to start
        while ((transform.position - _initialPosition).sqrMagnitude > 0.000001f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                _initialPosition,
                _recoilSpeed * Time.deltaTime
            );
            yield return null;
        }

        transform.position = _initialPosition;
        _recoilCoroutine = null;
    }
}
