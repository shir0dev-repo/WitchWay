using UnityEngine;
using UnityEngine.EventSystems;

public class DropSpot : MonoBehaviour
{
    [SerializeField] private string correctObjID;
    [SerializeField] private GameObject sparkle;
    private bool isCorrect = false;

    public void OnDrop(PointerEventData eventData)
    {
        //if (isCorrect) return;

        SortingObjects obj = eventData.pointerDrag?.GetComponent<SortingObjects>();
        if (obj != null && !isCorrect)
        {
            float dist = Vector2.Distance(obj.transform.position, transform.position);
            if (dist < 1000f && obj.objID == correctObjID)
            {
                obj.transform.SetParent(transform);
                obj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                isCorrect = true;
                obj.PlacedCorrectly();

                Instantiate(sparkle, transform.position, Quaternion.identity, transform);
            }
            else
            {
                obj.ResetObjPos();
            }
        }
    }
}
