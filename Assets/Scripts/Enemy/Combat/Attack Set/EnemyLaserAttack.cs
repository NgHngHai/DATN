using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemyLaserAttack : EnemyAttackBehavior
{
    [Header("Attack: Laser Beam")]
    [SerializeField] private LayerMask obstacleLayers;
    [SerializeField] private LineRenderer laserLineRenderer;
    [SerializeField] private HurtBox hurtBox;
    [SerializeField] private float laserLength = 20f;
    [SerializeField] private float laserDuration = 0.2f;

    private Collider2D hurtBoxCollider;
    private Coroutine attackRoutine;

    private void Awake()
    {
        hurtBoxCollider = hurtBox.GetComponent<Collider2D>();
        hurtBoxCollider.enabled = false;
    }

    protected override void Attack()
    {
        if (attackRoutine != null)
            StopCoroutine(attackRoutine);

        attackRoutine = StartCoroutine(DoLaserAttack());
    }

    private IEnumerator DoLaserAttack()
    {
        Vector3 start = attackPoint.position;
        Vector3 dir = attackPoint.right;

        RaycastHit2D hit = Physics2D.Raycast(start, dir, laserLength, obstacleLayers);
        Vector3 end = hit ? (Vector3)hit.point : start + dir * laserLength;

        ShowLaser(start, end);
        EnableLaserCollider(start, end, dir);

        yield return new WaitForSeconds(laserDuration);

        hurtBoxCollider.enabled = false;
        laserLineRenderer.enabled = false;
    }

    private void EnableLaserCollider(Vector3 start, Vector3 end, Vector3 dir)
    {
        if (hurtBoxCollider is BoxCollider2D box)
        {
            box.enabled = true;

            float length = Vector2.Distance(start, end);
            box.size = new Vector2(length, laserLineRenderer.startWidth);

            box.transform.right = dir;

            float offsetX = (length / 2f) * Mathf.Sign(dir.x);
            box.offset = new Vector2(offsetX, 0);
        }
    }

    private void ShowLaser(Vector3 start, Vector3 end)
    {
        laserLineRenderer.enabled = true;
        laserLineRenderer.SetPosition(0, start);
        laserLineRenderer.SetPosition(1, end);
    }

    public override bool IsTargetInAttackArea(Transform target)
    {
        if (target == null) return false;

        Vector2 start = attackPoint.position;
        Vector2 dir = (target.position - attackPoint.position).normalized;
        float distance = Vector2.Distance(start, target.position);

        if (distance > laserLength)
            return false;

        return !PhysicsUtils.IsRaycastHit(start, dir, distance, obstacleLayers);
    }
}
