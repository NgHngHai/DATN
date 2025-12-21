using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : GenericSingleton<AudioManager>
{
    public const string MASTER_VOLUME_KEY = "Master_Volume";
    public const string MUSIC_VOLUME_KEY = "Music_Volume";
    public const string SFX_VOLUME_KEY = "SFX_Volume";

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;

    [Header("Background Music")]
    [SerializeField] private AudioSource musicSource;

    [Header("Sound Effect (Pool)")]
    [SerializeField] private GameObject presetSfxPrefab;
    [SerializeField] private int sfxPoolSize = 12;

    private List<AudioSource> sfxSources = new List<AudioSource>();
    public float masterVolume = 1f;
    public float musicVolume = 1f;
    public float sfxVolume = 1f;

    protected override void Awake()
    {
        base.Awake();

        InitializeAllAudioSources();
        InitializeStoredAudioVolume();
    }

    //      ============================================
    //      ==================  INIT  ==================
    //      ============================================
    private void InitializeAllAudioSources()
    {
        if (musicSource == null)
        {
            GameObject musicObject = new GameObject("BGM_Source");
            musicObject.transform.parent = transform;
            musicSource = musicObject.AddComponent<AudioSource>();
            musicSource.outputAudioMixerGroup = musicGroup;
            musicSource.loop = true;
        }

        if (presetSfxPrefab != null)
        {
            for (int i = 0; i < sfxPoolSize; i++)
            {
                GameObject sfxObj = Instantiate(presetSfxPrefab, transform);
                AudioSource sfxSource = sfxObj.GetComponent<AudioSource>();
                sfxSources.Add(sfxSource);
            }
        }
        else
        {
            for (int i = 0; i < sfxPoolSize; i++)
            {
                GameObject sfxObj = new GameObject("SFX_Source (No Preset)");
                sfxObj.transform.parent = transform;

                AudioSource sfxSource = sfxObj.AddComponent<AudioSource>();
                sfxSource.outputAudioMixerGroup = sfxGroup;
                sfxSource.playOnAwake = false;
                sfxSources.Add(sfxSource);
            }
        }
    }

    private void InitializeStoredAudioVolume()
    {
        SetMasterVolume(PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, masterVolume));
        SetMusicVolume(PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, musicVolume));
        SetSFXVolume(sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, sfxVolume));
    }


    //      =============================================
    //      ==================  MUSIC  ==================
    //      =============================================
    public void PlayBackgroundMusic(AudioClip clip)
    {
        if (clip == null) return;

        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopBackgroundMusic() => musicSource.Stop();


    //      ===========================================
    //      ==================  SFX  ==================
    //      ===========================================
    public void PlaySFX(AudioClip clip, float volume = 1, float pitchMin = 0, float pitchMax = 0)
    {
        if (clip == null) return;

        AudioSource src = GetFreeSFXSource();

        src.clip = clip;
        src.volume = volume;
        src.pitch = Random.Range(pitchMin, pitchMax);

        src.spatialBlend = 0f;
        src.Play();
    }

    public void PlaySFXAt(AudioClip clip, Vector3 position, float volume = 1, float pitchMin = 0, float pitchMax = 0)
    {
        if (clip == null) return;

        AudioSource src = GetFreeSFXSource();

        src.clip = clip;
        src.transform.position = position;
        src.volume = volume;
        src.pitch = Random.Range(pitchMin, pitchMax);

        src.spatialBlend = 1f;
        src.Play();
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


    //      ===================================================
    //      ==================  AUDIO MIXER  ==================
    //      ===================================================
    public void SetMasterVolume(float value01)
    {
        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, value01);
        audioMixer.SetFloat(MASTER_VOLUME_KEY, LinearToDecibel(value01));
    }

    public void SetMusicVolume(float value01)
    {
        musicVolume = value01;
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, value01);
        audioMixer.SetFloat(MUSIC_VOLUME_KEY, LinearToDecibel(value01));
    }

    public void SetSFXVolume(float value01)
    {
        sfxVolume = value01;
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, value01);
        audioMixer.SetFloat(SFX_VOLUME_KEY, LinearToDecibel(value01));
    }

    public float GetMasterVolume() => masterVolume;
    public float GetMusicVolume() => musicVolume;
    public float GetSFXVolume() => sfxVolume;

    private float LinearToDecibel(float value)
    {
        return value <= 0.0001f ? -80f : Mathf.Log10(value) * 20f;
    }
}
