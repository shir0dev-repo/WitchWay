using System.Collections.Generic;
using UnityEngine;

public class IngredientsInPot : MonoBehaviour
{
    public List<WorldIngredient> IngredientsToAdd = new();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ingredient"))
        {
            if (other.TryGetComponent(out WorldIngredient ing))
            {
                GameEvents.Crafting.OnItemPlacedInCauldron?.Invoke(ing);
                IngredientsToAdd.Add(ing);
            }
                
            other.gameObject.SetActive(false);
        }
    }
    public void ReturnRejectedIngredients()
    {
        if (IngredientsToAdd == null) {  return; }

        foreach(WorldIngredient i in IngredientsToAdd)
        {
            i.gameObject.SetActive(true);
            i.gameObject.transform.position = new Vector3(-3,0,0);
            i.gameObject.transform.rotation = Quaternion.identity;
        }

        ClearList();
    }
    public void UseIngredientsInValidRecipe()
    {
        foreach (WorldIngredient i in IngredientsToAdd)
        {
            Destroy(i.gameObject);
        }

        ClearList();
    }
    public void ClearList()
    {
        IngredientsToAdd?.Clear();
    }
    public List<WorldIngredient> GetIngredients() {  return IngredientsToAdd; }
}
