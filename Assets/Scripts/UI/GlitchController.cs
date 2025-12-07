using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class GlitchController : MonoBehaviour
{
    public float glitchSpeed = 1f;
    public GameObject ScreenCanvas;
    Renderer rend;
    MaterialPropertyBlock mpb;
    float uValueInMap;
    float glitchFactor = 0.001f;
    int dir = 1;

    void Start()
    {
        rend = GetComponent<Renderer>();
        mpb = new MaterialPropertyBlock();
        uValueInMap = Random.Range(0, 5) * 0.2f + 0.1f;
    }


    public void StartSceneTransition()
    {
        StartCoroutine(SceneTransitionCoroutine());
    }
    IEnumerator SceneTransitionCoroutine()
    {
        glitchFactor = 0.001f; dir = 1;
        ScreenCanvas.SetActive(false);
        yield return new WaitForSeconds(0.3f);

        while (dir != -1)
        {
            glitchFactor += Time.deltaTime * glitchSpeed * dir;
            if (glitchFactor >= 0.5f) dir = -1;
            rend.GetPropertyBlock(mpb);
            mpb.SetFloat("_MapUV_u", 0.1f);
            mpb.SetFloat("_GlitchFactor", glitchFactor);
            rend.SetPropertyBlock(mpb);
            yield return null;
        }

        while (glitchFactor > 0f)
        {
            glitchFactor += Time.deltaTime * glitchSpeed * dir;
            if (glitchFactor < 0f) glitchFactor = 0f;
            rend.GetPropertyBlock(mpb);
            mpb.SetFloat("_MapUV_u", 0.1f);
            mpb.SetFloat("_GlitchFactor", glitchFactor);
            rend.SetPropertyBlock(mpb);
            yield return null;
        }

        // while (glitchFactor < 0f)
        // {
        //     glitchFactor += Time.deltaTime * glitchSpeed * dir;
        //     if (glitchFactor > 0f) glitchFactor = 0f;
        //     rend.GetPropertyBlock(mpb);
        //     mpb.SetFloat("_MapUV_u", 0.1f);
        //     mpb.SetFloat("_GlitchFactor", glitchFactor);
        //     rend.SetPropertyBlock(mpb);
        //     yield return null;
        // }

        yield return new WaitForSeconds(0.75f);

        // while (glitchFactor < )
    }
}