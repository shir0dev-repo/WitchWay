using UnityEngine;

[System.Serializable]
public class ChanceIngredient
{
    public IngredientSO Ingredient;
    [Range(0, 100)] public int Chance;
    public float ObjectScale = 1;
}
