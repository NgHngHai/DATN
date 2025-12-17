using UnityEngine;

public class EntityAnimationEvent : MonoBehaviour
{
    protected Entity entity;

    protected virtual void Awake()
    {
        entity = GetComponentInParent<Entity>();
    }

    public void CallCurrentAnimationTrigger()
    {
        entity.CallCurrentAnimationStateTrigger();
    }
}
