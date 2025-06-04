using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RecipeManager : MonoBehaviour
{
    [SerializeField] private List<SortingObjects> recipeObjs;
    [SerializeField] private Image recipeImg;   // Uh I wasnt sure if this would be an image or text, should be easy to just swap it
    private int count = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        recipeImg.gameObject.SetActive(false);
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
}
