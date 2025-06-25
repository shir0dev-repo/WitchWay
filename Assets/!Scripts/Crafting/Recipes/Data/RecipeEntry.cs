using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RecipeEntry
{
    public IngredientSO TargetIngredient;
    public List<CraftingOperation> OrderedCraftingOperations;
}
