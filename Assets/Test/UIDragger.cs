using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIDragger : MonoBehaviour, IDragHandler
{
    public Canvas canvas;
    public CanvasScaler canvasScaler;
    public RectTransform rectTransform;//Image的RectTransform
    private float ScaleFactor => Screen.width / canvasScaler.referenceResolution.x * (1 - canvasScaler.matchWidthOrHeight) + Screen.height / canvasScaler.referenceResolution.y * canvasScaler.matchWidthOrHeight;

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition = eventData.position / ScaleFactor;
        //rectTransform.anchoredPosition = eventData.position;
        //rectTransform.position = eventData.position;//这种方式对SceenSpace_Camera无效
        //rectTransform.anchoredPosition = eventData.position + canvasScaler.referenceResolution;
    }
}
