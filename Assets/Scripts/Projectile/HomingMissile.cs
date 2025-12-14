using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HomingMissile : Projectile
{
    [Header("Homing Missile")]
    [SerializeField] private LayerMask homingMask;
    [SerializeField] private float rotateSpeed = 200f;   
    [SerializeField] private float homingRadius = 6f;
    [SerializeField] private float moveSpeed = 5f;

    private Transform followTarget;
    private Vector2 moveDirection;

    private void Start()
    {
        FindTargetWithinRadius();
        moveDirection = transform.up;
    }

    private void Update()
    {
        Vector2 desiredDirection = moveDirection;

        if (followTarget != null)
        {
            desiredDirection = ((Vector2)followTarget.position - rb.position).normalized;
            moveDirection = desiredDirection;
        }

        float angle = Vector2.SignedAngle(transform.up, desiredDirection);
        float maxRotate = rotateSpeed * Time.deltaTime;
        float rotateAmount = Mathf.Clamp(angle, -maxRotate, maxRotate);

        rb.MoveRotation(rb.rotation + rotateAmount);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = transform.up * moveSpeed;
    }

    private void FindTargetWithinRadius()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, homingRadius, homingMask);

        if (hits.Length == 0)
        {
            followTarget = null;
            return;
        }

        float minDist = float.MaxValue;
        Transform bestTarget = null;

        foreach (Collider2D col in hits)
        {
            float dist = Vector2.Distance(transform.position, col.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                bestTarget = col.transform;
            }
        }

        followTarget = bestTarget;
    }

    private void OnDestroy()
    {
        if (alreadyHit) return;
        CreateHitEffect(transform.position);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, homingRadius);
    }
}
