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

    private readonly List<AudioSource> sfxSources = new();

    private float masterVolume = 1f;
    private float musicVolume = 1f;
    private float sfxVolume = 1f;

    protected override void Awake()
    {
        base.Awake();
        InitializeAllAudioSources();
        LoadVolumes();
        ApplyAllMixerVolumes();
    }

    // ================= INIT =================
    private void InitializeAllAudioSources()
    {
        if (musicSource == null)
        {
            var go = new GameObject("BGM_Source");
            go.transform.SetParent(transform);
            musicSource = go.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.outputAudioMixerGroup = musicGroup;
        }

        for (int i = 0; i < sfxPoolSize; i++)
        {
            AudioSource src;

            if (presetSfxPrefab != null)
            {
                src = Instantiate(presetSfxPrefab, transform).GetComponent<AudioSource>();
            }
            else
            {
                var go = new GameObject("SFX_Source");
                go.transform.SetParent(transform);
                src = go.AddComponent<AudioSource>();
                src.playOnAwake = false;
                src.outputAudioMixerGroup = sfxGroup;
            }

            sfxSources.Add(src);
        }
    }



    // ================= MUSIC =================
    public void PlayBackgroundMusic(AudioClip clip)
    {
        if (clip == null) return;
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopBackgroundMusic() => musicSource.Stop();

    // ================= SFX =================
    public void PlaySFX(AudioClip clip, float volume = 1, float pitchMin = 0.9f, float pitchMax = 1.1f)
    {
        if (clip == null) return;

        AudioSource src = GetFreeSFXSource();
        src.clip = clip;
        src.volume = volume;
        src.pitch = Random.Range(pitchMin, pitchMax);
        src.spatialBlend = 0f; // 2D Sound
        src.Play();
    }

    public void PlaySFXAt(AudioClip clip, Vector3 position, float volume = 1, float pitchMin = 0.9f, float pitchMax = 1.1f)
    {
        if (clip == null) return;

        AudioSource src = GetFreeSFXSource();
        src.clip = clip;
        src.transform.position = position;
        src.volume = volume;
        src.pitch = Random.Range(pitchMin, pitchMax);
        src.spatialBlend = 1f; // 3D Sound
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

    // ================= VOLUME API =================
    public void SetMasterVolume(float value)
    {
        masterVolume = value;
        audioMixer.SetFloat(MASTER_VOLUME_KEY, LinearToDecibel(value));
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        audioMixer.SetFloat(MUSIC_VOLUME_KEY, LinearToDecibel(value));
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        audioMixer.SetFloat(SFX_VOLUME_KEY, LinearToDecibel(value));
    }

    public float GetMasterVolume() => masterVolume;
    public float GetMusicVolume() => musicVolume;
    public float GetSFXVolume() => sfxVolume;

    private void LoadVolumes()
    {
        masterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, 1f);
        musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
        sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
    }

    public void SaveVolumes()
    {
        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, masterVolume);
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicVolume);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxVolume);
        PlayerPrefs.Save();
    }

    private void ApplyAllMixerVolumes()
    {
        audioMixer.SetFloat(MASTER_VOLUME_KEY, LinearToDecibel(masterVolume));
        audioMixer.SetFloat(MUSIC_VOLUME_KEY, LinearToDecibel(musicVolume));
        audioMixer.SetFloat(SFX_VOLUME_KEY, LinearToDecibel(sfxVolume));
    }

    private float LinearToDecibel(float value)
    {
        return value <= 0.0001f ? -80f : Mathf.Log10(value) * 20f;
    }
}
