using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Alchemy/New Recipe")]
public class RecipeSO : ScriptableObject
{
    public List<RecipeEntry> Ingredients;
    public PotionData Output;
    public CauldronEffectParams CauldronEffects;
    public bool IsDiscovered;

    /*
    STEPS:
        - Check if counts are the same
        - Compare ingredient modifiers to recipe entry
     */
    public bool IsValidRecipe(List<ModifiedIngredient> selectedIngredients)
    {
        if (selectedIngredients.Count != Ingredients.Count) return false;

        int count = Ingredients.Count;

        // --- OBTAIN RECIPE INGREDIENT COUNT ---
        var ingredientChecklist = CreateChecklist(count);

        // --- OBTAIN SELECTED INGREDIENT COUNT ---
        for (int i = 0; i < count; i++)
        {
            IngredientSO ingredient = selectedIngredients[i].BaseIngredient;
            if (ingredientChecklist.ContainsKey(ingredient))
            {
                var tuple = ingredientChecklist[ingredient];
                tuple.count++;
                ingredientChecklist[ingredient] = tuple;
            }
            else // wrong ingredient passed into cauldron
            {
                return false;
            }
        }

        // not correct amount of ingredients
        if (ingredientChecklist.Values.Any(tuple => tuple.target != tuple.count))
            return false;

        // collect and compare recipe entries to given ingredients
        foreach (IngredientSO ingredient in ingredientChecklist.Keys)
        {
            // find all occurrences of modified ingredients matching this recipe
            List<RecipeEntry> entries = Ingredients.Where(entry => entry.TargetIngredient == ingredient).ToList();
            List<ModifiedIngredient> matchingIngredients = selectedIngredients.Where(ing => ing.BaseIngredient == ingredient).ToList();

            if (entries.Count != matchingIngredients.Count) return false;

            foreach (ModifiedIngredient ing in matchingIngredients)
            {
                // find matching entry, remove it or return false
                RecipeEntry match = entries.FirstOrDefault(entry => ing.ModifierList.OperationsPerformed.SequenceEqual(entry.OrderedCraftingOperations));

                if (match == null) return false;

                // found match, remove from entries
                entries.Remove(match);
            }
        }

        return true;
    }

    private Dictionary<IngredientSO, (int target, int count)> CreateChecklist(int count)
    {
        Dictionary<IngredientSO, (int target, int count)> checklist = new();
        for (int i = 0; i < count; i++)
        {
            IngredientSO targetIngredient = Ingredients[i].TargetIngredient;
            if (!checklist.ContainsKey(targetIngredient))
            {
                checklist.Add(targetIngredient, (1, 0));
            }
            else
            {
                var tuple = checklist[targetIngredient];
                tuple.target++;
                checklist[targetIngredient] = tuple;
            }
        }

        return checklist;
    }
}
