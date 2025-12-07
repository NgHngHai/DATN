using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WipeButtonController : MonoBehaviour
{
    Image image;
    Material imageMaterialInstance;
    float wipeProgress = 0.09f;
    Coroutine wipeCoroutine;
    public float wipeSpeed = 0.2f;

    void Start()
    {
        image = GetComponent<Image>();
        imageMaterialInstance = new Material(image.material);
        image.material = imageMaterialInstance;
    }

    public void StartWipeEffect()
    {
        wipeCoroutine = StartCoroutine(WipeEffectCoroutine());
    }
    IEnumerator WipeEffectCoroutine()
    {
        while (wipeProgress < 1f)
        {
            wipeProgress += wipeSpeed * Time.deltaTime;
            imageMaterialInstance.SetFloat("_WipeProgress", wipeProgress);
            yield return null;
        }
    }

    public void StopWipeEffect()
    {
        if (wipeCoroutine != null)
        {
            StopCoroutine(wipeCoroutine);
        }
        wipeProgress = 0.09f;
        imageMaterialInstance.SetFloat("_WipeProgress", wipeProgress);
    }
}
