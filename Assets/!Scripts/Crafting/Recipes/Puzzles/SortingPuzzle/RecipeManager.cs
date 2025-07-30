using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RecipeManager : MonoBehaviour
{
    [SerializeField] private List<SortingObjects> recipeObjs;
    [SerializeField] private Image recipeImg;   // Uh I wasnt sure if this would be an image or text, should be easy to just swap it
    private int count = 0;
    [SerializeField] private string recipeID;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bool recipeLearned = !string.IsNullOrEmpty(recipeID) && SaveManager.Instance.hasRecipe(recipeID);
        if (recipeLearned)
        {
            //SetSortingActive(false);
            ShowLearnedRecipe();
        }
        else
        {
            recipeImg.gameObject.SetActive(false);
            SetSortingActive(false);
        }
    }

    public void AddCount()
    {
        count++;
        if (count >= recipeObjs.Count)
        {
            StartCoroutine(ShowRecipe());
        }
    }
    private IEnumerator ShowRecipe()
    {
        foreach (var obj in recipeObjs)
        {
            StartCoroutine(FadeOut(obj.GetComponent<CanvasGroup>()));
        }
        yield return new WaitForSeconds(1f);

        recipeImg.gameObject.SetActive(true);
        CanvasGroup imgGroup = recipeImg.GetComponent<CanvasGroup>();
        if (imgGroup != null)
        {
            StartCoroutine(FadeIn(imgGroup));
        }
        // yield return null;
        if (!string.IsNullOrEmpty(recipeID))
        {
            SaveManager.Instance.LearnRecipe(recipeID);
        }
    }
    private IEnumerator FadeOut(CanvasGroup group)
    {
        float duration = 1f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            group.alpha = Mathf.Lerp(1f, 0f, time / duration);
            yield return null;
        }

        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;
    }
    private IEnumerator FadeIn(CanvasGroup group)
    {
        float duration = 1.5f;
        float time = 0f;
        group.alpha = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            group.alpha = Mathf.Lerp(0f, 1f, time / duration);
            yield return null;
        }

        group.alpha = 1f;
    }
    public void SetSortingActive(bool active)
    {
        foreach (var obj in recipeObjs)
        {
            obj.enabled = active;
            obj.gameObject.SetActive(active);
        }
    }
    private void ShowLearnedRecipe()
    {
        recipeImg.gameObject.SetActive(true);
        CanvasGroup imgGroup = recipeImg.GetComponent<CanvasGroup>();
        if (imgGroup != null)
        {
            imgGroup.alpha = 1f;
            imgGroup.interactable = true;
            imgGroup.blocksRaycasts = true;
        }
        foreach (var obj in recipeObjs)
        {
            obj.PleaseHideTheseIdiots();
        }

    }
}
