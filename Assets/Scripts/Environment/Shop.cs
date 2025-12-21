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
        UIManager.Instance.OpenShop();
    }

    private void OnDisable()
    {
        // If we get disabled (leaving scene, etc.), make sure dialog isn't left open
        if (dialogUI != null && dialogUI.IsOpen)
            dialogUI.Hide();

        _dialogOpen = false;
    }
}