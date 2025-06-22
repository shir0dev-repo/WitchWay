using System.Collections.Generic;
using UnityEngine;

public class RecipeList : MonoBehaviour
{
    public List<RecipeSO> RecipeSOList;
    // has to be set in inspector
    
    public List<RecipeSO> filterResultsBySingleIngredient(IngredientSO ingredient)
    {
        List <RecipeSO> list = new List <RecipeSO>();

        foreach (RecipeSO s in RecipeSOList)
        {
            foreach (RecipeEntry r in s.Ingredients)
            {
                if (r.TargetIngredient == ingredient)
                {
                    list.Add(s);
                }
            }
        }

        return list;
        // can tweak this later
    }
    public List<RecipeSO> GetAllRecipes()
    {
        return RecipeSOList;
    } 
}
