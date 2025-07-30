using FMODUnity;
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

    [Header("Sound")]
    [SerializeField] private EventReference onPictureMoveSound;

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

        transform.SetAsLastSibling();
    }
    public void OnDrag(PointerEventData evenData)
    {
        if (isPlacedCorrectly) return;

        rectTransform.anchoredPosition += evenData.delta / canvas.scaleFactor;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        SoundManager.Instance.PlayOneShot(onPictureMoveSound);
        Debug.Log("the OnPictureMoveSound would play here if it existed");

        canvasGroup.blocksRaycasts = true;
        if (isPlacedCorrectly) { return; }

        DropSpot[] dropSpots = FindObjectsOfType<DropSpot>();
        foreach (DropSpot spot in dropSpots)    //arf
        {
            if (spot.IsCorrect) { continue; }

            RectTransform spotRect = spot.GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(spotRect, Input.mousePosition, canvas.worldCamera))
            {
                if (spot.CorrectObjID == objID)
                {
                    rectTransform.position = spotRect.position;
                    isPlacedCorrectly = true;
                    spot.FlagCorrect();
                    PlacedCorrectly();
                    return;
                }
            }
        }
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
        canvasGroup.blocksRaycasts = false;
        transform.SetAsFirstSibling();      // did anyone else know this existed? I feel so silly
    }

    public void PleaseHideTheseIdiots()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        gameObject.SetActive(false);
    }
}
