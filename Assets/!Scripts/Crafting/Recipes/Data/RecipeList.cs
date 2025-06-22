using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecipeList : MonoBehaviour
{
    public List<RecipeSO> RecipeSOList;
    // has to be set in inspector
    
    public List<RecipeSO> FilterResultsBySingleIngredient(WorldIngredient worldIngredient)
    {
        IngredientSO ingredient = worldIngredient.BaseIngredient;

        List <RecipeSO> list = new List <RecipeSO>();
        IEnumerable<RecipeSO> result = RecipeSOList.Where(i => i.Ingredients.Any(j => j.TargetIngredient.Equals(ingredient)));

        foreach (RecipeSO s in result)
        {
            list.Add(s);
        }

        return list;
    }
    public List<RecipeSO> FilterResultsByMultipleIngredients(List<WorldIngredient> worldIngredients)
    {
        List <RecipeSO> list = new List <RecipeSO>();
        List<IngredientSO> ingredients = new List<IngredientSO>();

        foreach(WorldIngredient w in worldIngredients)
        {
            ingredients.Add(w.BaseIngredient);
        }

        var ingredientHash = new HashSet<IngredientSO>(ingredients.Select(i => i));
        IEnumerable<RecipeSO> result = RecipeSOList.Where(i => i.Ingredients.Any(j => ingredientHash.All(k => j.TargetIngredient.Equals(ingredientHash))));         

        foreach (RecipeSO s in result)
        {
            list.Add(s);
        }

        return list;
    }
    public List<RecipeSO> GetAllRecipes()
    {
         return RecipeSOList;
    } 
    List<IngredientSO> GetAllIngredients(RecipeSO chosenRecipe)
    {
        List<IngredientSO> list = new List <IngredientSO>();

        foreach(RecipeEntry e in chosenRecipe.Ingredients)
        {
            list.Add(e.TargetIngredient);
        }

        return list;
    }
}
