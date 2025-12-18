using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SpriteRenderer))]
public class SplashSceneController : MonoBehaviour
{
    public GameObject presentsText;
    public MainMenuOverlay overlayController;
    [Header("Materials")]
    public Material normalMaterial;
    public Material alternativeMaterial;
    [Header("First Effect Factors")]
    public float firstEffectDuration;
    public float firstEffectDelay;
    [Header("Second Effect Factors")]
    public float transitionDelay;
    public float secondEffectDuration;
    public float secondEffectHoldTime;
    public float secondEffectDelay;

    AsyncOperation loadSceneOperation;
    #region Variables used for shader
    Vector4 distortionMapST;
    float logoRatio, randomOffsetU, randomOffsetV;
    SpriteRenderer rend;
    MaterialPropertyBlock mpb;
    static readonly int DistortionMapST_ID = Shader.PropertyToID("_DistortionMapST");
    static readonly int LogoRatioID = Shader.PropertyToID("_MainTexUVRatio");
    static readonly int RandomOffsetU_ID = Shader.PropertyToID("_RandomOffsetU");
    static readonly int RandomOffsetV_ID = Shader.PropertyToID("_RandomOffsetV");
    static readonly int AlphaValueID = Shader.PropertyToID("_AlphaValue");
    #endregion


    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        mpb = new MaterialPropertyBlock();
        loadSceneOperation = SceneManager.LoadSceneAsync(1);
        loadSceneOperation.allowSceneActivation = false;
        StartCoroutine(ApplyEffect());
    }
    

    IEnumerator ApplyEffect()
    {
        float timer = 0;
        WaitForSeconds wfsFirstEffectDelay = new(firstEffectDelay);
        WaitForSeconds wfsSecondEffectDelay = new(secondEffectDelay);
        while(timer < firstEffectDuration)
        {
            distortionMapST = new(Random.Range(0.1f, 0.3f), Random.Range(0.1f, 0.3f), Random.Range(0, 1), Random.Range(0, 1));
            logoRatio = Random.Range(0.2f, 0.5f);

            rend.GetPropertyBlock(mpb);
            mpb.SetVector(DistortionMapST_ID, distortionMapST);
            mpb.SetFloat(LogoRatioID, logoRatio);
            rend.SetPropertyBlock(mpb);

            timer += firstEffectDelay;
            yield return wfsFirstEffectDelay;
        }

        distortionMapST = new(0.125f, 0.125f, 0.875f, 0f);
        rend.GetPropertyBlock(mpb);
        mpb.SetVector(DistortionMapST_ID, distortionMapST);
        mpb.SetFloat(LogoRatioID, logoRatio);
        rend.SetPropertyBlock(mpb);

        yield return new WaitForSeconds(transitionDelay);

        rend.sharedMaterial = alternativeMaterial;
        mpb.Clear();

        timer = secondEffectDuration;
        while (timer > 0)
        {
            randomOffsetU = Random.Range(-0.1f, 0.1f);
            randomOffsetV = Random.Range(-0.1f, 0.1f);

            rend.GetPropertyBlock(mpb);
            mpb.SetFloat(RandomOffsetU_ID, randomOffsetU);
            mpb.SetFloat(RandomOffsetV_ID, randomOffsetV);
            mpb.SetFloat(AlphaValueID, 1 - timer / secondEffectDuration);
            rend.SetPropertyBlock(mpb);

            timer -= secondEffectDelay;
            if (timer < 0) timer = 0;
            yield return wfsSecondEffectDelay;
        }
        
        rend.sharedMaterial = normalMaterial;
        presentsText.SetActive(true);

        while (secondEffectHoldTime > 0)
        {
            secondEffectHoldTime -= Time.deltaTime;
            yield return null;
        }

        rend.sharedMaterial = alternativeMaterial;

        while (timer < secondEffectDuration)
        {
            randomOffsetU = Random.Range(-0.1f, 0.1f);
            randomOffsetV = Random.Range(-0.1f, 0.1f);

            rend.GetPropertyBlock(mpb);
            mpb.SetFloat(RandomOffsetU_ID, randomOffsetU);
            mpb.SetFloat(RandomOffsetV_ID, randomOffsetV);
            mpb.SetFloat(AlphaValueID, 1- timer / secondEffectDuration);
            rend.SetPropertyBlock(mpb);

            timer += secondEffectDelay;
            yield return wfsSecondEffectDelay;
        }

        mpb.SetFloat(AlphaValueID, 0);
        rend.SetPropertyBlock(mpb);
        presentsText.SetActive(false);

        yield return new WaitForSeconds(0.5f);
        overlayController.ActivateTransition();
        loadSceneOperation.allowSceneActivation = true;
    }
}
