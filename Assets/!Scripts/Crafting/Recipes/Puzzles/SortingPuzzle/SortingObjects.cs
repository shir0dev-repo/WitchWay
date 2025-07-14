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

    [Header("Image Stuff")]
    private Image imageComponent;
    [SerializeField] private Sprite correctSprite;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        imageComponent = GetComponent<Image>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isPlacedCorrectly) return;

        originPos = rectTransform.anchoredPosition;
        canvasGroup.blocksRaycasts = false;
    }
    public void OnDrag(PointerEventData evenData)
    {
        if (isPlacedCorrectly) return;

        rectTransform.anchoredPosition += evenData.delta / canvas.scaleFactor;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (isPlacedCorrectly) return;

        canvasGroup.blocksRaycasts = true;
        // if (!eventData.pointerEnter || !eventData.pointerEnter.GetComponent<DropSpot>())
        //{
        //    rectTransform.anchoredPosition = originPos;
        //  }

    }
    public void PlacedCorrectly()
    {
        isPlacedCorrectly = true;
        //canvasGroup.interactable = false;
        // canvasGroup.blocksRaycasts = false;

        if (correctSprite != null && imageComponent != null)
        {
            imageComponent.sprite = correctSprite;
        }
    }
}
