using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AudioSlider : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private enum AudioGroupType
    {
        Master,
        Music,
        SFX
    }

    [SerializeField] private AudioGroupType audioControlType;

    [Header("Fill Area")]
    [SerializeField] private Image fillImage;
    [SerializeField] private Color selectedColor;

    [Header("Volume Text")]
    [SerializeField] private TextMeshProUGUI audioValueTMP;
    [SerializeField] private int maxValue = 20;

    private Color startColor;
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        startColor = fillImage.color;
        InitSliderValue();
    }

    private void InitSliderValue()
    {
        float initValue = 1f;

        switch (audioControlType)
        {
            case AudioGroupType.Music:
                initValue = AudioManager.Instance.GetMusicVolume();
                break;
            case AudioGroupType.SFX:
                initValue = AudioManager.Instance.GetSFXVolume();
                break;
            default:
                initValue = AudioManager.Instance.GetMasterVolume();
                break;
        }

        slider.value = initValue;
        audioValueTMP.text = Mathf.FloorToInt(initValue * maxValue).ToString();
    }

    private void OnEnable()
    {
        slider.onValueChanged.AddListener(OnSliderChanged);
    }

    private void OnDisable()
    {
        slider.onValueChanged.RemoveListener(OnSliderChanged);
        AudioManager.Instance.SaveVolumes();
    }

    private void OnSliderChanged(float value)
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

        audioValueTMP.text = Mathf.FloorToInt(value * maxValue).ToString();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        fillImage.color = selectedColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        fillImage.color = startColor;
    }
}
