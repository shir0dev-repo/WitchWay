using System.Collections.Generic;
using UnityEngine;

public class StockedPotions : Singleton<StockedPotions>
{
    public readonly Dictionary<PotionData, int> Stock = new();
    [SerializeField] private List<PotionData> _potions = new();

    private void OnEnable()
    {
        GameEvents.Crafting.OnPotionFullyCompleted += AddPotion;
    }

    private void OnDisable()
    {
        GameEvents.Crafting.OnPotionFullyCompleted -= AddPotion;
    }

    private void AddPotion(PotionData potion)
    {
        if (Stock.ContainsKey(potion))
        {
            Stock[potion] = Stock[potion] + 1;
        }
        else
        {
            Stock.Add(potion, 1);
        }

        _potions.Add(potion);
    }
    public void AddPotion(Potion potion)
    {
        AddPotion(potion.Data);
    }

    private void RemovePotion(PotionData data)
    {
        if (Stock.TryGetValue(data, out int count))
        {
            if (--count <= 0)
                Stock.Remove(data);
            else
            {
                Stock[data] = count - 1;
            }
        }
    }

    public bool SellPotion(PotionEffect type)
    {
        foreach (var d in Stock)
        {
            if (d.Key.Effect == type)
            {
                RemovePotion(d.Key);
                return true;
            }
        }

        return false;
    }
}
