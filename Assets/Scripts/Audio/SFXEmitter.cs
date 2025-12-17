using UnityEngine;

public class SFXEmitter : MonoBehaviour
{
    [SerializeField] private AudioClip[] sfxClips;
    [Range(0f, 1f)]
    [SerializeField] private float volume = 1f;
    [SerializeField] private float pitchMin = 0.9f;
    [SerializeField] private float pitchMax = 1.1f;

    private AudioClip currentSfxClip;

    private void Awake()
    {
        currentSfxClip = sfxClips[0];
    }

    /// <summary>
    /// Updates the selected clip index and plays it as a non-spatial (2D) sound.
    /// </summary>
    /// <param name="sfxClipIndex">The index of the clip in the sfxClips array.</param>
    public void ChangeAndPlaySFX(int sfxClipIndex)
    {
        ChangeCurrentSFX(sfxClipIndex);
        PlaySFX();
    }

    /// <summary>
    /// Updates the selected clip index and plays it at the current object's world position (3D).
    /// </summary>
    /// <param name="sfxClipIndex">The index of the clip in the sfxClips array.</param>
    public void ChangeAndPlaySFXAtHere(int sfxClipIndex)
    {
        ChangeCurrentSFX(sfxClipIndex);
        PlaySFXAtHere();
    }

    /// <summary>
    /// Sets the <see cref="currentSfxClip"/> based on the provided array index.
    /// </summary>
    /// <param name="sfxClipIndex">The index of the target audio clip.</param>
    public void ChangeCurrentSFX(int sfxClipIndex)
    {
        currentSfxClip = sfxClips[sfxClipIndex];
    }

    /// <summary>
    /// Triggers the current audio clip globally via the <see cref="AudioManager"/>.
    /// </summary>
    public void PlaySFX()
    {
        AudioManager.Instance.PlaySFX(currentSfxClip, volume, pitchMin, pitchMax);
    }

    /// <summary>
    /// Triggers the current audio clip at this object's transform position via the <see cref="AudioManager"/>.
    /// </summary>
    public void PlaySFXAtHere()
    {
        AudioManager.Instance.PlaySFXAt(currentSfxClip, transform.position, volume, pitchMin, pitchMax);
    }
}
