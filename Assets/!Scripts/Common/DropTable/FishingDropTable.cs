using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class DropTableItem
{
    public IngredientSO Item;
    [SerializeField, Range(0, 100)] public int Chance;
}

[System.Serializable]
public class FishingDropTable
{
    [SerializeField] private List<DropTableItem> _possibleDrops = new();

    public FishingRod.Result GetDrop(out IngredientSO drop)
    {
        drop = null;
        
        float chance = Random.Range(0, 101);
        if (chance == 1)
        {
            // do jumpscare things

            return FishingRod.Result.Jumpscare;
        }

        var possible = _possibleDrops
            .Where(d => chance <= d.Chance)
            .Select(d => d.Item)
            .ToList();

        if (possible.Count <= 0) return FishingRod.Result.Nothing;

        drop = possible[Random.Range(0, possible.Count)];
        return FishingRod.Result.Item;
    }
}
