using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Alchemy/New Recipe")]
public class RecipeSO : ScriptableObject
{
    public List<RecipeEntry> Ingredients;
    public PotionData Output;
}
