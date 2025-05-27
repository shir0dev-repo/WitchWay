using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

}
