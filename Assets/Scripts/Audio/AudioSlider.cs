using UnityEngine;
using UnityEngine.UI;

public class AudioSlider : MonoBehaviour
{
    enum AudioGroupType
    {
        Master,
        Music,
        SFX
    }

    [SerializeField] private AudioGroupType audioControlType;
    private Slider audioSlider;
    private string audioKey;

    private void Awake()
    {
        audioSlider = GetComponent<Slider>();

        switch (audioControlType)
        {
            case AudioGroupType.Music:
                audioKey = AudioManager.MUSIC_VOLUME_KEY;
                break;
            case AudioGroupType.SFX:
                audioKey = AudioManager.SFX_VOLUME_KEY;
                break;
            default:
                audioKey = AudioManager.MASTER_VOLUME_KEY;
                break;
        }

        audioSlider.value = PlayerPrefs.GetFloat(audioKey, 1);
    }

    private void OnEnable()
    {
        audioSlider.onValueChanged.AddListener(OnSliderChange);
    }

    private void OnDisable()
    {
        audioSlider.onValueChanged.RemoveListener(OnSliderChange);
    }

    private void OnSliderChange(float value)
    {
        switch (audioControlType)
        {
            case AudioGroupType.Music:
                AudioManager.Instance.SetMusicVolume(value);
                break;
            case AudioGroupType.SFX:
                AudioManager.Instance.SetSFXVolume(value);
                break;
            default:
                AudioManager.Instance.SetMasterVolume(value);
                break;
        }
    }
}
