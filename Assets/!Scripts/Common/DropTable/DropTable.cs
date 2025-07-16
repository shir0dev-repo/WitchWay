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
public class DropTable
{
    [SerializeField] private List<DropTableItem> _possibleDrops = new();

    public IngredientSO GetDrop()
    {
        float chance = Random.Range(0, 101);
        if (chance == 1)
        {
            // do jumpscare things
            return null;
        }

        var possible = _possibleDrops
            .Where(d => chance <= d.Chance)
            .Select(d => d.Item)
            .ToList();

        if (possible.Count <= 0) return null;

        return possible[Random.Range(0, possible.Count)];
    }
}
