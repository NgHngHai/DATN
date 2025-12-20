using System;
using UnityEngine;

public class SnatcherSecondaryCollider : MonoBehaviour
{
    [SerializeField] private HurtBox hurtBox;
    [SerializeField] private LayerMask landMask;

    public event Action OnLandingSuccess;

    private bool hasJumpedOff;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(hasJumpedOff && Utility.IsGameObjectInLayer(collision.gameObject, landMask))
        {
            OnLandingSuccess?.Invoke();
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!hasJumpedOff && Utility.IsGameObjectInLayer(collision.gameObject, landMask))
        {
            hasJumpedOff = true;
        }
    }
}
