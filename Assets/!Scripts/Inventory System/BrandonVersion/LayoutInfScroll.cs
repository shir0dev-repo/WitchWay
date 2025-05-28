using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//abandoned for now as im just making it clamp
public class LayoutInfScroll : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform viewportTransform;
    [SerializeField] private RectTransform contentTransform;
    [SerializeField] private LayoutGroup layoutGroup; //should be able to use any

    private List<RectTransform> itemsList = new List<RectTransform>();

    public void UpdateItemsList()
    {
        for (int i = 0; i < contentTransform.childCount; i++)
        {
            itemsList.Add(contentTransform.GetChild(i).GetComponent<RectTransform>());
        }
    }
}
