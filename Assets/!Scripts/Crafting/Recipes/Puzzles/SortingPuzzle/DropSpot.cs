using UnityEngine;
using UnityEngine.EventSystems;

public class DropSpot : MonoBehaviour, IDropHandler
{
    [SerializeField] private string correctObjID;
    //[SerializeField] private GameObject sparkle;
    [SerializeField] private RecipeManager recipeManager;
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
                //obj.transform.SetParent(transform);
                // obj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                RectTransform objTrans = obj.GetComponent<RectTransform>();
                RectTransform targetTrans = GetComponent<RectTransform>();

                objTrans.position = targetTrans.position;

                isCorrect = true;
                obj.PlacedCorrectly();

                // Instantiate(sparkle, transform.position, Quaternion.identity, transform);

                recipeManager.AddCount();
            }
        }
    }
}
