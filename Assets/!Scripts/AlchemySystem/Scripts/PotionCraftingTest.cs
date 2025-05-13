using UnityEngine;

public class PotionCraftingTest : MonoBehaviour
{
    public IngredientSO[] _ingredients;

    private void Start()
    {
        Potion p = PotionFactory.BrewPotion(_ingredients);
    }
}
