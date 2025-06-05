using UnityEngine;
using System;

public static partial class GameEvents
{
    /// <summary>Events related to crafting stations and the player's inventory.</summary>
    public static class Crafting
    {
        public static Action<int> OnStationChanged;

        public static Action<IngredientSO> OnItemAddedToInventory;
        public static Action<IngredientSO> OnItemRemovedFromInventory;
        public static Action<WorldIngredient> OnItemPlacedInTrash;

        public static Action<WorldIngredient> OnItemPlacedOnCuttingBoard;
        public static Action<WorldIngredient> OnCutItem;

        public static Action<WorldIngredient> OnItemPlacedInMortar;
        public static Action<WorldIngredient, float> OnItemDurabilityChanged;

        public static Action<WorldIngredient> OnItemPlacedInArcaneCircle;
        public static Action<AlchemicalSymbol> OnSymbolDrawn;


        public static Action<WorldIngredient> OnItemPlacedInCauldron;
        public static Action OnCauldronMixStepCompleted;
        public static Action<Potion> OnCauldronFullyMixed;

        public static Action OnBottleSelected;
        public static Action OnBottlingStationSelected;

        public static Action<RecipeSO> OnRecipeUnlocked;
    }
}
