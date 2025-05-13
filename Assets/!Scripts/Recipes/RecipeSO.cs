using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Alchemy/New Recipe")]
public class RecipeSO : ScriptableObject
{
    public IngredientSO[] Ingredients;
    public Potion Output;

    [ContextMenu("CalculateCost")]
    public void CalculateCost()
    {
        
    }
}
