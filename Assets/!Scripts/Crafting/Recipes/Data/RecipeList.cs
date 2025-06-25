using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecipeList : MonoBehaviour
{
    public List<RecipeSO> RecipeSOList;
    // has to be set in inspector
    public RecipeSO GetFirstRecipeFromListofMultiple(List<WorldIngredient> world)
    {
        return FilterResultsByMultipleIngredients(world).FirstOrDefault();
        // will change this later so it will pick from the most applicable actions that need to be performed on each ingredient
    }
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
        // create a hashset with all the requested ingredients
        IEnumerable<RecipeSO> result = RecipeSOList
            .Where(recipeSO => recipeSO.Ingredients
                .Any(entry => ingredientHash.Any(filter => entry.TargetIngredient.Equals(filter))));
        // IT SEARCHES LIKE THIS: recipeSOlist -> gets list of recipeEntry -> grabs the ingredientSO in each recipeEntry
        // (cont.) -> compares the ingredients to the ingredientHash -> adds recipeSO to the IEnum if all ingredients are present.
        // iterates this process over the entirety of the recipeSOList so it kinda sucks but it works...
        list.AddRange(result);
        
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
