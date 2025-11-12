using UnityEngine;

[DisallowMultipleComponent]
public class Explosion : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float activeTime = 0.1f;
    [SerializeField] private float destroyDelay = 0.2f;

    private HurtBox hurtBox;

    private void Awake()
    {
        hurtBox = GetComponent<HurtBox>();
    }

    private void OnEnable()
    {
        Invoke(nameof(DisableHurtBox), activeTime);
        Destroy(gameObject, destroyDelay);
    }

    private void DisableHurtBox()
    {
        hurtBox.ToggleHurtCollider(false);
    }
}
