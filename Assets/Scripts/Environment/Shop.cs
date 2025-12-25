using TMPro;
using UnityEngine;

public class Shop : Interactables
{
    [SerializeField] private GameObject tutotialPromptEN;
    private SFXEmitter sfxEmitter;

    protected override void Awake()
    {
        base.Awake();
        sfxEmitter = GetComponent<SFXEmitter>();
    }

    protected override void OnInteract(GameObject player)
    {
        sfxEmitter.ChangeAndPlaySFXAtHere(0); // Shop open SFX
        UIManager.Instance.OpenShop();
    }

    new private void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        tutotialPromptEN.SetActive(true);
    }

    new private void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
        tutotialPromptEN.SetActive(false);
    }
}