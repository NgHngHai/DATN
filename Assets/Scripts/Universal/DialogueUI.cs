using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class DialogUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Root GameObject for the dialog (typically the panel).")]
    [SerializeField] private GameObject root;

    [Tooltip("Dialog message text.")]
    [SerializeField] private TMP_Text messageText;

    [Header("Buttons")]
    [SerializeField] private Button positiveButton;
    [SerializeField] private TMP_Text positiveLabel;

    [SerializeField] private Button negativeButton;
    [SerializeField] private TMP_Text negativeLabel;

    private Action _onPositive;
    private Action _onNegative;

    public bool IsOpen => root != null && root.activeSelf;

    private void Awake()
    {
        if (root == null) root = gameObject;
        Hide();
    }

    public void Show(string message, string positiveText, string negativeText, Action onPositive, Action onNegative)
    {
        if (root == null) return;

        _onPositive = onPositive;
        _onNegative = onNegative;

        if (messageText != null) messageText.text = message ?? string.Empty;
        if (positiveLabel != null) positiveLabel.text = string.IsNullOrEmpty(positiveText) ? "OK" : positiveText;
        if (negativeLabel != null) negativeLabel.text = string.IsNullOrEmpty(negativeText) ? "Cancel" : negativeText;

        positiveButton.onClick.RemoveAllListeners();
        negativeButton.onClick.RemoveAllListeners();

        if (positiveButton != null)
            positiveButton.onClick.AddListener(() =>
            {
                _onPositive?.Invoke();
                Hide();
            });

        if (negativeButton != null)
            negativeButton.onClick.AddListener(() =>
            {
                _onNegative?.Invoke();
                Hide();
            });

        root.SetActive(true);
    }

    public void Hide()
    {
        if (root != null) root.SetActive(false);
        _onPositive = null;
        _onNegative = null;

        if (positiveButton != null) positiveButton.onClick.RemoveAllListeners();
        if (negativeButton != null) negativeButton.onClick.RemoveAllListeners();
    }
}