using UnityEngine;

public class HomingMissile : Projectile
{
    [Header("Homing Missile")]
    [SerializeField] private LayerMask homingMask;
    [SerializeField] private float rotateSpeed = 200f;
    [SerializeField] private float homingRadius = 6f;

    private Transform followTarget;
    private float moveSpeed = 5f;

    private void Start()
    {
        //moveSpeed = rb.linearVelocity.magnitude;
        FindTargetWithinRadius();
    }

    private void FixedUpdate()
    {

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, homingRadius);
    }
}
