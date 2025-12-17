using UnityEngine;
using UnityEngine.EventSystems;

public class SkillButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public SkillTreeController skillTreeController;
    GameObject buttonSelectorGo;

    void Awake()
    {
        buttonSelectorGo = transform.parent.GetChild(0).gameObject;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (skillTreeController.IsSelectingSkill(transform.parent.GetSiblingIndex())) return;
        skillTreeController.DisplayData(transform.parent.GetSiblingIndex());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonSelectorGo.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (skillTreeController.IsSelectingSkill(transform.parent.GetSiblingIndex())) return;
        buttonSelectorGo.SetActive(false);
    }
}
