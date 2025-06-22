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
    public List<WorldIngredient> GetIngredients() {  return IngredientsToAdd; }
}
