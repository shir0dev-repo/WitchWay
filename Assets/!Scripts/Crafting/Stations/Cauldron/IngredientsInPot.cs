using System.Collections.Generic;
using UnityEngine;

public class IngredientsInPot : MonoBehaviour
{
    public List<GameObject> IngredientsToAdd = new();

    private void Update()
    {
        if (StationManager.Instance.GetCurrentStation() ==3 && Input.GetKeyDown(KeyCode.Space))
        {
            SwitchToMixing.ActivateMixing?.Invoke();
        }
        // press the spacebar to start mixing WHEN ITS ON THE CORRECT STATION 
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ingredient"))
        {
            IngredientsToAdd.Add(other.gameObject);

            if (other.TryGetComponent(out WorldIngredient ing))
                GameEvents.Crafting.OnItemPlacedInCauldron?.Invoke(ing);
        }
    }

}
