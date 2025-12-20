using System.Collections;
using TMPro;
using UnityEngine;

public class TypewriterEffect : MonoBehaviour
{
    [SerializeField] private TMP_Text _textComponent;
    [SerializeField] private float _delayPerChar = 0.05f;
    
    WaitForSeconds wfsDelayPerChar;
    Coroutine _typingCoroutine;

    void Awake()
    {
        wfsDelayPerChar = new(_delayPerChar);
    }

    void OnEnable()
    {
        ShowText("Heh?");
    }

    public void ShowText(string fullContent)
    {
        _textComponent.text = fullContent;
        _textComponent.ForceMeshUpdate();
        _textComponent.maxVisibleCharacters = 0;

        if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
        _typingCoroutine = StartCoroutine(TypingRoutine());
    }

    IEnumerator TypingRoutine()
    {
        int totalChars = _textComponent.textInfo.characterCount;

        for (int i = 0; i <= totalChars; i++)
        {
            _textComponent.maxVisibleCharacters = i;
            // AudioManager.Play("TypeSound");

            yield return wfsDelayPerChar;
        }
    }
}