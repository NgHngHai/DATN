using System.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(PauseButtonAnimating))]
public class PauseButtonController : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    Coroutine enterButtonCoroutine, exitButtonCoroutine;
    PauseButtonAnimating animator;
    public int buttonIndex = 0;
    public float effectSpeed = 2;

    void Start()
    {
        animator = GetComponent<PauseButtonAnimating>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (buttonIndex == 0)
        {
            animator.SetEffectFactor(0);
            // UIManager.Instance.ResumePlaying();
            Time.timeScale = 1;
            transform.parent.parent.gameObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (enterButtonCoroutine == null)
        {
            enterButtonCoroutine = StartCoroutine(EnterButtonCoroutine());
            if (exitButtonCoroutine != null)
            {
                StopCoroutine(exitButtonCoroutine);
                exitButtonCoroutine = null;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (exitButtonCoroutine == null)
        {
            exitButtonCoroutine = StartCoroutine(ExitButtonCoroutine());
            if (enterButtonCoroutine != null)
            {
                StopCoroutine(enterButtonCoroutine);
                enterButtonCoroutine = null;
            }
        }
    }


    IEnumerator EnterButtonCoroutine()
    {
        while (animator.effectFactor < 1f)
        {
            float f = animator.effectFactor + effectSpeed * Time.deltaTime;
            if (f > 1f) f = 1f;
            animator.SetEffectFactor(f);
            yield return null;
        }
        enterButtonCoroutine = null;
    }


    IEnumerator ExitButtonCoroutine()
    {
        while (animator.effectFactor > 0f)
        {
            float f = animator.effectFactor - effectSpeed * Time.deltaTime;
            if (f < 0f) f = 0f;
            animator.SetEffectFactor(f);
            yield return null;
        }
        exitButtonCoroutine = null;
    }
}
