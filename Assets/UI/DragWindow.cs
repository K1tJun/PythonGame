using UnityEngine;
using UnityEngine.EventSystems;

public class WindowDrag : MonoBehaviour, IDragHandler
{
    public RectTransform window;

    public void OnDrag(PointerEventData eventData)
    {
        window.anchoredPosition += eventData.delta;
    }
}
















