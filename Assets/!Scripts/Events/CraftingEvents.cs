using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class GameEvents
{
    /// <summary>Events related to crafting stations and the player's inventory.</summary>
    public static class Crafting
    {
        // General Station events
        public static Action<int> OnStationChanged;
        public static Action<ToolType> OnToolSelected;
        public static Action<ToolType> OnToolDeselected;

        // Cursor Events
        public static Action<IFollowCursor> OnObjectAttachedToCursor;
        public static Action<IFollowCursor> OnObjectRemovedFromCursor;

        // Inventory Events
        public static Action<IngredientSO> OnItemAddedToInventory;
        public static Action<IngredientSO> OnItemRemovedFromInventory;

        public static Action<ModifiedIngredient> OnIngredientUIHover;
        public static Action OnIngredientUIUnhovered;
        
        public static Action<WorldIngredient> OnItemPlacedInTrash;
        

        public static Action<WorldIngredient, StationType, Transform> OnItemPlacedInStation;


        // Cutting Board Events
        public static Action<WorldIngredient> OnItemPlacedOnCuttingBoard;
        public static Action<WorldIngredient> OnItemRemovedFromCuttingBoard;

        public static Action<WorldIngredient> OnSuccessfullyCutItem;
        public static Action<WorldIngredient> OnFailedToCutItem;

        public static Action<WorldIngredient, Transform> OnCutItem;

        // Mortar and Pestle Events
        public static Action<WorldIngredient> OnItemPlacedInMortar;
        public static Action<WorldIngredient> OnItemRemovedFromMortar;

        public static Action<WorldIngredient> OnSuccessfullyCrushedItem;
        public static Action<WorldIngredient> OnFailedToCrushItem;

        public static Action<WorldIngredient, float> OnItemDurabilityChanged;

        // Temperature Pot events
        public static Action<WorldIngredient> OnItemPlacedInTemperaturePot;
        public static Action<WorldIngredient> OnItemRemovedFromTemperaturePot;

        public static Action<WorldIngredient> OnItemSuccessfullyFrozen;
        public static Action<WorldIngredient> OnFailedToFreezeItem;

        public static Action<WorldIngredient> OnItemSuccessfullyHeated;
        public static Action<WorldIngredient> OnFailedToHeatItem;

        // Arcane Circle events
        public static Action<WorldIngredient> OnItemPlacedInArcaneCircle;
        public static Action<WorldIngredient> OnItemRemovedFromArcaneCircle;

        public static Action<WorldIngredient, AlchemicalSymbol> OnItemSuccessfullySpellbound;
        public static Action<WorldIngredient> OnFailedToSpellbindItem;

        public static Action<AlchemicalSymbol> OnSymbolDrawn;

        // Cauldron Events
        public static Action<WorldIngredient> OnItemPlacedInCauldron;
        public static Action OnCauldronMixStepCompleted;
        public static Action OnCauldronMixSequenceCompleted;

        public static Action<List<ModifiedIngredient>, bool> OnMixedPotionRequested;

        public static Action<PotionData> OnSuccessfullyMixedPotion;
        public static Action<RecipeSO> OnFailedToMixPotion;


        // Bottler events
        // TODO: Add Bottle class as parameter
        public static Action OnBottleSelected;
        public static Action<Bottle> OnBottlePlacedInBottler;
        public static Action OnBottlingStationSelected;
        public static Action<float> OnBottleFillChanged;
        public static Action OnBottleFilled;

        // Recipe events
        public static Action<RecipeSO> OnRecipeUnlocked;
    }
}
