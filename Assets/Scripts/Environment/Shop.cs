using TMPro;
using UnityEngine;

public class Shop : Interactables
{
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
}