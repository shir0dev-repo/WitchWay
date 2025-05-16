using System.Collections.Generic;
using Shir0.InventorySystem;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ingredient", menuName = "Alchemy/New Ingredient")]
public class IngredientSO : ItemData
{
    [Space]
    [Range(0.5f, 5.0f)] public float CostMultiplier = 1.0f;
    public PotionEffect PotionEffect;
}