using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Alchemy/New Recipe")]
public class RecipeSO : ScriptableObject
{
    public ModifiedIngredient[] Ingredients;
    public Potion Output;
}
