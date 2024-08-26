using UnityEngine;
using UnityEngine.EventSystems;

public class ConsoleDrugAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private CanvasGroup _canvasGroup;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.blocksRaycasts = true;
        }
    }
}