using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Flags]
public enum PotionEffect 
{
    None = 0,
    Health = 1,
    Strength = 2,
    Stamina = 4,
    EVIL = 8,
    Truth = 16,
    Invisibility = 32
}

public static class PotionFactory
{
    private static Dictionary<PotionEffect, int> _effectCostDictionary = new()
    {
        { PotionEffect.None, 0 },
        { PotionEffect.Health, 12 },
        { PotionEffect.Strength, 18 },
        { PotionEffect.Stamina, 24 },
        { PotionEffect.EVIL, 30 },
        { PotionEffect.Truth, 36 },
        { PotionEffect.Invisibility, 42 },
    };

    public static Potion BrewPotion(params IngredientSO[] ingredients)
    {
        Potion p = new Potion();
        (p.PrimaryEffect, p.SecondaryEffect) = CalculateEffects(ingredients);
        p.Cost = CalculateCost(_effectCostDictionary[p.PrimaryEffect], ingredients);

        Debug.Log($"Primary: {p.PrimaryEffect}, Secondary: {p.SecondaryEffect}, Cost: {p.Cost}g");
        return p;
    }

    public static int CalculateCost(int baseCost, params IngredientSO[] ingredients)
    {
        float costMultiplier = 1.0f;

        foreach (IngredientSO i in ingredients)
        {
            costMultiplier *= i.CostMultiplier;
        }

        return Mathf.RoundToInt(baseCost * costMultiplier);
    }

    private static (PotionEffect primary, PotionEffect secondary) CalculateEffects(params IngredientSO[] ingredients)
    {
        List<(PotionEffect effect, int count)> effectCounts = new();
        
        foreach (IngredientSO i in ingredients)
        {
            PotionEffect e = i.PotionEffect;

            int effectIndex = effectCounts.FindIndex(v => v.effect == e);
            if (effectIndex == -1)
            {
                effectCounts.Add((e, 1));
            }
            else
            {
                effectCounts[effectIndex] = (e, effectCounts[effectIndex].count + 1);
            }
        }

        effectCounts = effectCounts.OrderByDescending(v => v.count).ToList();
        PotionEffect primaryEffect = effectCounts[0].effect;

        PotionEffect secondaryEffect = effectCounts[1].effect;

        return (primaryEffect, secondaryEffect);
    }
}
