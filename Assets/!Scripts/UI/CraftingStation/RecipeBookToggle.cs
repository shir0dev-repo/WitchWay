using UnityEngine;
using System.Collections;
using FMODUnity;

public class RecipeBookToggle : MonoBehaviour
{
    [SerializeField] private float animationTime = 0.1f;
    [SerializeField] private RectTransform recipeBookPanel;

    [Header("Sound")]
    [SerializeField] private EventReference bookOpenSound, bookCloseSound;

    private Vector2 startPos, targetPos;

    private RecipeManager recMan;

    void Start()
    {
        if (StationManager.Instance != null)
            StationManager.Instance.OnStationChanged.AddListener(OnStationChanged);

        startPos = recipeBookPanel.anchoredPosition;

        recMan = FindObjectOfType<RecipeManager>();   // I hope this works when we add additional recipies... I guess I could test it... Oh well!
    }

    public void ToggleRecipeBook()
    {

        if (StationManager.Instance.recipeBookOpen)
        {
            StationManager.Instance.recipeBookOpen = false;
            SoundManager.Instance.PlayOneShot(bookCloseSound);
            CursorManager.BlockInteraction = false;
            recMan.SetSortingActive(false);
        }
        else
        {
            StationManager.Instance.recipeBookOpen = true;
            CursorManager.BlockInteraction = true;
            SoundManager.Instance.PlayOneShot(bookOpenSound);
            recMan.SetSortingActive(true);
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
