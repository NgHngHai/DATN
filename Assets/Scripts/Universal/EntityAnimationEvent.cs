using UnityEngine;

public class EntityAnimationEvent : MonoBehaviour
{
    protected Entity entity;

    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
    }

    public void CallCurrentAnimationTrigger()
    {
        entity.CallCurrentAnimationStateTrigger();
    }

    public void PlaySFX(AudioClip clip)
    {
        AudioManager.Instance.PlaySFX(clip);
    }

    public void PlaySFXAtHere(AudioClip clip)
    {
        AudioManager.Instance.PlaySFXAt(clip, transform.position);
    }
}
