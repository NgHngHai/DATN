using TMPro;
using UnityEngine;

public class Shop : Interactables
{
    [Header("Shop Dialog")]
    [Tooltip("Dialog UI instance on a screen-space canvas.")]
    [SerializeField] private DialogUI dialogUI;

    [Tooltip("Optional: The Shop UI root to enable when opening the shop. If null, will try tag 'ShopUI'.")]
    [SerializeField] private GameObject shopUIRoot;

    [TextArea]
    [SerializeField] private string dialogMessage = "Welcome! Do you want to open the shop?";

    [SerializeField] private string openButtonText = "Open Shop";
    [SerializeField] private string quitButtonText = "Quit";

    private bool _dialogOpen;

    protected override void OnInteract(GameObject player)
    {
        if (_dialogOpen) return;

        EnsureReferences();

        // Show 2-option dialog
        if (dialogUI != null)
        {
            _dialogOpen = true;
            HideWorldSpacePromptIfPresent();

            dialogUI.Show(
                dialogMessage,
                openButtonText,
                quitButtonText,
                onPositive: OpenShop,
                onNegative: CloseDialog
            );
        }
    }

    private void OpenShop()
    {
        _dialogOpen = false;

        // Enable the Shop UI
        if (shopUIRoot == null)
        {
            var tagged = GameObject.FindWithTag("ShopUI");
            if (tagged != null) shopUIRoot = tagged;
        }

        if (shopUIRoot != null)
        {
            shopUIRoot.SetActive(true);
        }
    }

    private void CloseDialog()
    {
        _dialogOpen = false;
    }

    private void EnsureReferences()
    {
        if (dialogUI == null)
        {
#if UNITY_2022_1_OR_NEWER
            dialogUI = FindFirstObjectByType<DialogUI>();
#else
            dialogUI = FindObjectOfType<DialogUI>();
#endif
        }
    }

    private static void HideWorldSpacePromptIfPresent()
    {
        // Optional: hide the shared world-space prompt while dialog is open
        var canvasObj = GameObject.FindWithTag("World Space Canvas");
        if (canvasObj == null) return;

        var c = canvasObj.GetComponent<Canvas>();
        var txt = canvasObj.GetComponentInChildren<TMP_Text>(true);
        if (txt != null) txt.gameObject.SetActive(false);
        if (c != null) c.enabled = false;
    }

    private void OnDisable()
    {
        // If we get disabled (leaving scene, etc.), make sure dialog isn't left open
        if (dialogUI != null && dialogUI.IsOpen)
            dialogUI.Hide();

        _dialogOpen = false;
    }
}