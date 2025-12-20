using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FileButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    Coroutine enterButtonCoroutine, exitButtonCoroutine;
    public Sprite selectedSprite, unselectedSprite;
    public FileSelection fileSelection;
    RectTransform btnRect;
    public float animationSpeed = 2f;
    public RectTransform rect;
    public Image img;
    public TextMeshProUGUI txtFileIndex, txtBigHeader, txtSmallHeader;
    public RectTransform rectFileIndex, rectMetadata;
    public GameObject goDeleteBtn;


    void Awake()
    {
        btnRect = GetComponent<RectTransform>();
    }

    void Onable()
    {
        StopFocusing();
    }

    public void Display(FileDisplayData displayData)
    {
        bool displayDataNull = displayData == null;
        txtBigHeader.text = displayDataNull ? "Start new game" : displayData.bigHeader;
        txtSmallHeader.text = displayDataNull ? string.Empty : displayData.smallHeader;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (fileSelection.IsSelectingFile(transform.GetSiblingIndex()))
        {
            fileSelection.StartPlayingGame();
        }
        else
        {
            fileSelection.ClickFileButton(transform.GetSiblingIndex());

        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (fileSelection.IsSelectingFile(transform.GetSiblingIndex())) return;
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
        if (fileSelection.IsSelectingFile(transform.GetSiblingIndex())) return;
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
        while (btnRect.sizeDelta.x < 1040)
        {
            btnRect.sizeDelta += new Vector2(animationSpeed * 120 * Time.deltaTime, 0);
            if (btnRect.sizeDelta.x > 1040) btnRect.sizeDelta = new Vector2(1040, btnRect.sizeDelta.y);
            rectFileIndex.anchoredPosition -= new Vector2(animationSpeed * 8 * Time.deltaTime, 0);
            if (rectFileIndex.anchoredPosition.x < -69) rectFileIndex.anchoredPosition = new Vector2(-69, rectFileIndex.anchoredPosition.y);
            yield return null;
        }
        enterButtonCoroutine = null;
    }


    IEnumerator ExitButtonCoroutine()
    {
        while (btnRect.sizeDelta.x > 920)
        {
            btnRect.sizeDelta -= new Vector2(animationSpeed * 120 * Time.deltaTime, 0);
            if (btnRect.sizeDelta.x < 920) btnRect.sizeDelta = new Vector2(920, btnRect.sizeDelta.y);
            rectFileIndex.anchoredPosition += new Vector2(animationSpeed * 8 * Time.deltaTime, 0);
            if (rectFileIndex.anchoredPosition.x > -61) rectFileIndex.anchoredPosition = new Vector2(-61, rectFileIndex.anchoredPosition.y);
            yield return null;
        }
        exitButtonCoroutine = null;
    }


    public void StopFocusing()
    {
        img.sprite = unselectedSprite;
        rect.anchoredPosition = new Vector2(0, 270 - 180 * transform.GetSiblingIndex());
        rect.sizeDelta = new Vector2(920, 155);
        txtFileIndex.color = Color.white;
        rectFileIndex.anchoredPosition = new Vector2(-61, 0);
        rectMetadata.anchoredPosition = new Vector2(80, 24);
        goDeleteBtn.SetActive(false);
    }


    public void StartFocusing()
    {
        img.sprite = selectedSprite;
        rect.anchoredPosition = new Vector2(-16, 270 - 180 * transform.GetSiblingIndex());
        rect.sizeDelta = new Vector2(1100, 155);
        txtFileIndex.color = Color.black;
        rectFileIndex.anchoredPosition = new Vector2(-69, 3);
        rectMetadata.anchoredPosition = new Vector2(115, 24);
        goDeleteBtn.SetActive(true);
    }
}
