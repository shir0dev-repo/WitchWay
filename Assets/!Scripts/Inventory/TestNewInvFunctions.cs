using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StationsDisplayIngredient
{
    public IngredientSO ingredient;
    public GameObject visualObject;

    public StationsDisplayIngredient(IngredientSO ingredient)
    {
        this.ingredient = ingredient;
    }
}

public class TestNewInvFunctions : MonoBehaviour
{
    [SerializeField] private IngredientSO[] ingredients;
    [SerializeField] private GameObject[] boxes;

    private GameObject lastClickedBox = null;

    void Start()
    {
        SortIntoBoxes();
    }

    void FixedUpdate()
    {
        CheckBoxClicked();
    }

    private void SortIntoBoxes()
    {
        foreach (GameObject box in boxes)
        {
            box.GetComponent<TestInvBox>().ClearItems();
        }

        //quantify by name
        Dictionary<string, BasketItems> groupedItems = new Dictionary<string, BasketItems>();
        foreach (IngredientSO ingred in ingredients)
        {
            if (groupedItems.ContainsKey(ingred.name))
            {
                groupedItems[ingred.name].itemAmount += 1;
            }
            else
            {
                groupedItems[ingred.name] = new BasketItems(null, ingred, 1);
            }
        }

        //distribute
        int[] boxCounts = new int[boxes.Length];

        foreach (BasketItems bValue in groupedItems.Values)
        {
            int remaining = bValue.itemAmount;
            int boxIndex = 0;

            while (remaining > 0)
            {
                //look for open box
                while (boxIndex < boxes.Length && boxCounts[boxIndex] >= 15) boxIndex++;

                if (boxIndex >= boxes.Length)
                {
                    Debug.LogWarning("to many for boxes");
                    return;
                }

                //calculate how many to assign
                int availableSpace = 15 - boxCounts[boxIndex];
                int assignAmount = Mathf.Min(availableSpace, remaining);
                boxCounts[boxIndex] += assignAmount;
                remaining -= assignAmount;

                //assign
                TestInvBox boxScript = boxes[boxIndex].GetComponent<TestInvBox>();
                boxScript.AddItem(new BasketItems(boxes[boxIndex].transform, bValue.assignedIngredient, assignAmount));
            }
        }
    }

    private void CheckBoxClicked()
    {
        foreach (GameObject box in boxes)
        {
            TestInvBox boxScript = box.GetComponent<TestInvBox>();

            if (boxScript.clicked)
            {
                boxScript.clicked = false;

                if (lastClickedBox != null && lastClickedBox != box)
                {
                    lastClickedBox.GetComponent<TestInvBox>().CloseIngredients();
                }

                if (lastClickedBox != box)
                {
                    boxScript.OpenIngredients();
                    lastClickedBox = box;
                }

                break;
            }
        }
    }
}
