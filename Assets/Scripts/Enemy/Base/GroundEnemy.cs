using UnityEngine;

/// <summary>
/// Extends <see cref="Enemy"/> with ground detection logic.
/// Handles ground checks, edge detection, and wall collisions for ground-based enemies.
/// </summary>
public abstract class GroundEnemy : Enemy
{
    [Header("Ground Enemy")]
    public LayerMask groundMask;
    [SerializeField] private Vector2 groundCheckLocalPos;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float groundEdgeCheckOffsetX;
    [SerializeField] private float wallCheckDistance;

    public bool IsGrounded()
    {
        return PhysicsUtils.IsRaycastHit(GetGroundCheckPos(), Vector2.down, groundCheckDistance, groundMask);
    }

    public bool IsGroundEdgeOrWallDetected()
    {
        Vector2 raycastDir = isFacingRight ? Vector2.right : Vector2.left;

        return PhysicsUtils.IsRaycastHit(transform.position, raycastDir, wallCheckDistance, groundMask) ||
            !PhysicsUtils.IsRaycastHit(GetGroundEdgeCheckPos(), Vector2.down, groundCheckDistance, groundMask);
    }

    Vector2 GetGroundCheckPos()
    {
        Vector2 normalizedPos = groundCheckLocalPos;
        normalizedPos.x *= FacingDir;
        return (Vector2)transform.position + normalizedPos;
    }

    Vector2 GetGroundEdgeCheckPos()
    {
        Vector2 normalizedPos = GetGroundCheckPos();
        normalizedPos.x = normalizedPos.x + groundEdgeCheckOffsetX * FacingDir;
        return normalizedPos;
    }

    public override void SetVelocity(Vector2 velocity)
    {
        if (!IsGrounded()) return;
        base.SetVelocity(velocity);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellowNice;

        Vector2 groundCheckPoint = GetGroundCheckPos();
        Gizmos.DrawLine(groundCheckPoint, groundCheckPoint + Vector2.down * groundCheckDistance);

        Vector2 groundEdgeCheckPos = GetGroundEdgeCheckPos();
        Gizmos.DrawLine(groundEdgeCheckPos, groundEdgeCheckPos + Vector2.down * groundCheckDistance);

        Vector3 raycastDir = isFacingRight ? Vector2.right : Vector2.left;
        Gizmos.DrawLine(transform.position, transform.position + raycastDir * wallCheckDistance);
    }
}
