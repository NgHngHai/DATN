using System.Collections;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    [Header("Laser Settings")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private HurtBox hurtBox;
    [SerializeField] private LayerMask obstacleLayers;

    private Collider2D hurtCollider;

    private void Awake()
    {
        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();
        if (hurtBox == null) hurtBox = GetComponentInChildren<HurtBox>();

        hurtCollider = hurtBox.GetComponent<Collider2D>();
        hurtBox.ToggleHurtCollider(false);

        lineRenderer.enabled = false;
    }

    public void Fire(Vector3 start, Vector3 direction, float maxLength, float duration)
    {
        StartCoroutine(FireRoutine(start, direction, maxLength, duration));
    }

    private IEnumerator FireRoutine(Vector3 start, Vector3 direction, float maxLength, float duration)
    {
        // Raycast to detect walls
        RaycastHit2D hit = Physics2D.Raycast(start, direction, maxLength, obstacleLayers);
        Vector3 end = hit ? (Vector3)hit.point : start + direction * maxLength;

        ShowLaser(start, end);
        EnableCollider(start, end, direction);

        yield return new WaitForSeconds(duration);

        hurtBox.ToggleHurtCollider(false);
        lineRenderer.enabled = false;

        Destroy(gameObject);
    }

    private void ShowLaser(Vector3 start, Vector3 end)
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    private void EnableCollider(Vector3 start, Vector3 end, Vector3 dir)
    {
        if (hurtCollider is BoxCollider2D box)
        {
            box.enabled = true;

            float length = Vector2.Distance(start, end);
            box.size = new Vector2(length, lineRenderer.startWidth);

            box.transform.right = dir;

            float offset = length / 2f;
            box.offset = new Vector2(offset, 0);
        }
    }
}
