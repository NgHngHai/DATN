using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuOverlay : MonoBehaviour
{
    public float duration1 = 2, duration2 = 2;
    Image overlay;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        overlay = transform.GetChild(0).GetComponent<Image>();
    }

    public void ActivateTransition()
    {
        gameObject.SetActive(true);
        StartCoroutine(SceneTransition());
    }
    IEnumerator SceneTransition()
    {
        WaitForSeconds wfsDelayTime = new(1);
        yield return wfsDelayTime;
        Color c = new(0, 0, 0, 1);
        float startTime = Time.realtimeSinceStartup;
        float duration = duration1;
        while (duration > 0)
        {
            c.a = 1f - 0.5f * Mathf.Sin((Time.realtimeSinceStartup - startTime) * Mathf.PI / duration1);
            overlay.color = c;
            duration -= Time.deltaTime;
            yield return null;
        }

        MenuManager menuManager = FindFirstObjectByType<MenuManager>();
        yield return null;
        duration = duration2;
        c.a = 1;
        overlay.color = c;
        menuManager.DisplayMenuButtons(true);
        yield return wfsDelayTime;
        
        startTime = Time.realtimeSinceStartup;
        while (duration > 0)
        {
            c.a = 1f - Mathf.Sin((Time.realtimeSinceStartup - startTime) * Mathf.PI / duration2 / 2);
            overlay.color = c;
            duration -= Time.deltaTime;
            yield return null;
        }
        c.a = 0f;
        overlay.color = c;
        Destroy(gameObject);
    }
}
