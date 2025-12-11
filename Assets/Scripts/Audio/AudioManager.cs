using System.Collections.Generic;
using UnityEngine;

public class AudioManager : GenericSingleton<AudioManager>
{
    [Header("Background Music")]
    [SerializeField] private AudioSource musicSource;
    public float musixVolume = 1f;

    [Header("Sound Effect (Pool)")]
    [SerializeField] private GameObject presetSfxPrefab;
    [SerializeField] private int sfxPoolSize = 12;
    public float sfxVolume = 1f;
    private List<AudioSource> sfxSources = new List<AudioSource>();

    protected override void Awake()
    {
        base.Awake();

        InitializeAllAudioSources();
    }

    public void PlayBackgroundMusic(AudioClip clip)
    {
        if (clip == null) return;

        musicSource.clip = clip;
        musicSource.volume = musixVolume;
        musicSource.Play();
    }

    public void StopBackgroundMusic() => musicSource.Stop();

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;

        AudioSource src = GetFreeSFXSource();

        src.spatialBlend = 0f;
        src.volume = sfxVolume;
        src.clip = clip;
        src.Play();
    }

    public void PlaySFXAt(AudioClip clip, Vector3 position)
    {
        if (clip == null) return;

        AudioSource src = GetFreeSFXSource();

        src.transform.position = position;
        src.spatialBlend = 1f;
        src.volume = sfxVolume;
        src.clip = clip;
        src.Play();
    }

    private void InitializeAllAudioSources()
    {
        if (musicSource == null)
        {
            GameObject bgmObj = new GameObject("BGM_Source");
            bgmObj.transform.parent = transform;
            musicSource = bgmObj.AddComponent<AudioSource>();
            musicSource.loop = true;
        }

        if (presetSfxPrefab != null)
        {
            for (int i = 0; i < sfxPoolSize; i++)
            {
                GameObject sfxObj = Instantiate(presetSfxPrefab, transform);
                AudioSource src = sfxObj.GetComponent<AudioSource>();
                sfxSources.Add(src);
            }
        }
        else
        {
            for (int i = 0; i < sfxPoolSize; i++)
            {
                GameObject sfxObj = new GameObject("SFX_Source (No Preset)");
                sfxObj.transform.parent = transform;

                AudioSource src = sfxObj.AddComponent<AudioSource>();
                src.playOnAwake = false;
                sfxSources.Add(src);
            }
        }
    }

    private AudioSource GetFreeSFXSource()
    {
        foreach (var src in sfxSources)
        {
            if (!src.isPlaying)
                return src;
        }

        return sfxSources[0];
    }
}
