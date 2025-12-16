using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuButtonController : MonoBehaviour
{
    Image image;
    Material imageMaterialInstance;
    float animatingProgress = 0f;
    Coroutine animatingCoroutine;
    public float animatingSpeed = 0.2f;

    void Start()
    {
        image = GetComponent<Image>();
        imageMaterialInstance = new Material(image.material);
        image.material = imageMaterialInstance;
    }

    public void StartWipeEffect()
    {
        animatingCoroutine = StartCoroutine(WipeEffectCoroutine());
    }
    IEnumerator WipeEffectCoroutine()
    {
        while (animatingProgress < 1f)
        {
            animatingProgress += animatingSpeed * Time.deltaTime;
            imageMaterialInstance.SetFloat("_Progress", animatingProgress);
            yield return null;
        }
    }

    public void StopWipeEffect()
    {
        if (animatingCoroutine != null)
        {
            StopCoroutine(animatingCoroutine);
        }
        animatingProgress = 0f;
        imageMaterialInstance.SetFloat("_Progress", animatingProgress);
    }
}
