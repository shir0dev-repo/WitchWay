using UnityEngine;

public class TestNewInvFunctions : MonoBehaviour
{
    [SerializeField] private IngredientSO[] ingredients;
    [SerializeField] private GameObject[] boxes;

    private GameObject lastClickedBox = null;

    void FixedUpdate()
    {
        CheckBoxClicked();
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
