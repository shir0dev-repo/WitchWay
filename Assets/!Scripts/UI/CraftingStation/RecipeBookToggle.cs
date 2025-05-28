using UnityEngine;
using System.Collections;

public class RecipeBookToggle : MonoBehaviour
{
    [SerializeField] private float animationTime = 0.1f;
    [SerializeField] private RectTransform recipeBookPanel;

    private Vector2 startPos, targetPos;

    void Start()
    {
        if (StationManager.Instance != null)
            StationManager.Instance.OnStationChanged.AddListener(OnStationChanged);

        startPos = recipeBookPanel.anchoredPosition;
    }

    public void ToggleRecipeBook()
    {
        
        if (StationManager.Instance.recipeBookOpen)
        {
            StationManager.Instance.recipeBookOpen = false;
        }
        else
        {
            StationManager.Instance.recipeBookOpen = true;
        }

        StartCoroutine(RecipeBookAnimation());
    }

    IEnumerator RecipeBookAnimation()
    {
        float elapsedTime = 0f;

        // the recipe book's open and closed position is just the y position flipped
        Vector2 startPos = recipeBookPanel.anchoredPosition;
        Vector2 targetPos = new Vector2(startPos.x, -startPos.y);

        while (elapsedTime < animationTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / animationTime);
            recipeBookPanel.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }
    }

    private void OnStationChanged(int index)
    {
        // close the recipe book if it is open when switching stations
        if (recipeBookPanel.anchoredPosition.y > 0)
        {
            ToggleRecipeBook();
        }
    }
        
}
