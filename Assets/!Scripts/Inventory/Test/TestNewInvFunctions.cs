using System.Collections.Generic;
using UnityEngine;

public class TestNewInvFunctions : MonoBehaviour
{
    [SerializeField] private IngredientSO[] ingredients;
    [SerializeField] private GameObject[] boxes;


    void Start()
    {
        SortIntoBoxes();
    }

    void FixedUpdate()
    {
    
    }

    private void SortIntoBoxes()
    {
        foreach (GameObject box in boxes)
        {
            box.GetComponent<TestInvBox>().ClearItems();
        }

        //quantify 
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

        //prep data
        int[] boxCounts = new int[boxes.Length];
        int currentBox = 0;

        foreach (BasketItems bValue in groupedItems.Values)
        {
            int remaining = bValue.itemAmount;

            while (remaining > 0)
            {
                int tries = 0;
                while (boxCounts[currentBox] >= 15 && tries < boxes.Length)
                {
                    currentBox = (currentBox + 1) % boxes.Length;
                    tries++;
                }

                if (tries >= boxes.Length)
                {
                    Debug.LogWarning("too many");
                    return;
                }

                TestInvBox boxScript = boxes[currentBox].GetComponent<TestInvBox>();
                boxScript.AddItem(new BasketItems(boxes[currentBox].transform, bValue.assignedIngredient, 1));

                boxCounts[currentBox]++;
                remaining--;

                currentBox = (currentBox + 1) % boxes.Length;
            }
        }
    }
}
