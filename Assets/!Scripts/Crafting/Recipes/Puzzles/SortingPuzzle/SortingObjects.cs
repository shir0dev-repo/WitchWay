using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SortingObjects : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 originPos;
    [SerializeField] public string objID;
    private bool isPlacedCorrectly = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originPos = rectTransform.anchoredPosition;
        canvasGroup.blocksRaycasts = false;
    }
    public void OnDrag(PointerEventData evenData)
    {
        rectTransform.anchoredPosition += evenData.delta / canvas.scaleFactor;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        // if (!eventData.pointerEnter || !eventData.pointerEnter.GetComponent<DropSpot>())
        //{
        //    rectTransform.anchoredPosition = originPos;
        //  }

    }
    public void PlacedCorrectly()
    {
        isPlacedCorrectly = true;
        canvasGroup.interactable = false;
    }
    public void ResetObjPos()
    {
        rectTransform.anchoredPosition = originPos;
        isPlacedCorrectly = false;
    }
}
