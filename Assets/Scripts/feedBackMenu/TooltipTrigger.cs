using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public LTDescr delay;
    public string header;
    [Multiline] public string content;

    public void OnPointerEnter(PointerEventData eventData)
    {
        delay = LeanTween.delayedCall(0.5f, () =>
        {
            Debug.Log("[TooltipTrigger] OnPointerEnter - Showing tooltip");
            TooltipSystem.Show(content, header);
        });
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.cancel(delay.uniqueId);
        Debug.Log("[TooltipTrigger] OnPointerExit - Hiding tooltip");
        TooltipSystem.Hide();
    }
}

