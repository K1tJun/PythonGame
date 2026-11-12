using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class HighlightButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
        [SerializeField] private float hoverScale = 1.1f;
        [SerializeField] private float duration = 0.2f;
    
        private RectTransform rect;
        private Vector3 firstScale;
    
        private void Awake()
        {
            rect = GetComponent<RectTransform>();
            firstScale = rect.localScale;
        }
    
        public void OnPointerEnter(PointerEventData eventData)
        {
            rect.DOScale(hoverScale, duration);
        }
    
        public void OnPointerExit(PointerEventData eventData)
        {
            rect.DOScale(firstScale, duration);
        }
        

        private void OnDisable()
        {
            rect.DOKill();
            rect.localScale = firstScale;
        }
}
