using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using FMODUnity;
using FMOD;

public class RecipePages : MonoBehaviour
{
    private List<RectTransform> pages = new List<RectTransform>();
    [SerializeField]
    private RectTransform pageContainer;
    private int currentPageIndex = 0;

    [Header("Page Navigation")]
    [SerializeField] private InputAction _changeStationAction;
    [SerializeField] private GameObject nextPageButton, previousPageButton;

    [Header("Sound")]
    [SerializeField] private EventReference pageTurnSound, pictureMoveSound;

    void Start()
    {
        foreach (Transform child in pageContainer)
        {
            if (child is RectTransform rectTransform)
            {
                pages.Add(rectTransform);
            }
        }

    }

    // stolen from station manager (thanks brandon)
    private void OnEnable()
    {
        _changeStationAction.started += MoveToStation;
        _changeStationAction.Enable();
    }

    private void OnDisable()
    {
        _changeStationAction.started -= MoveToStation;
        _changeStationAction.Disable();
    }

    public void NextPage()
    {
        currentPageIndex = (currentPageIndex + 1) % pages.Count;
        ShowPage(currentPageIndex);
        SoundManager.Instance.PlayOneShot(pageTurnSound);
    }

    public void PreviousPage()
    {
        currentPageIndex--;

        if (currentPageIndex < 0)
            currentPageIndex = pages.Count - 1;

        ShowPage(currentPageIndex);
        SoundManager.Instance.PlayOneShot(pageTurnSound);

    }

    private void ShowPage(int index)
    {
        // Hide buttons when not needed
        if (index == 0)
        {
            nextPageButton.SetActive(true);
            previousPageButton.SetActive(false);
        }
        else if (index == pages.Count - 1)
        {
            previousPageButton.SetActive(true);
            nextPageButton.SetActive(false);
        }
        else
        {
            previousPageButton.SetActive(true);
            nextPageButton.SetActive(true);
        }

        for (int i = 0; i < pages.Count; i++)
        {
            if (i == index)
            {
                pages[i].gameObject.SetActive(true);
            }
            else
            {
                pages[i].gameObject.SetActive(false);
            }
        }
    }

    private void MoveToStation(InputAction.CallbackContext context)
    {
        //dont change pages if the recipe book is not open
        if (StationManager.Instance.recipeBookOpen == false)
            return;


        float input = context.ReadValue<float>();

        if (input < 0)
            PreviousPage();
        else if (input > 0)
            NextPage();
    }
}
