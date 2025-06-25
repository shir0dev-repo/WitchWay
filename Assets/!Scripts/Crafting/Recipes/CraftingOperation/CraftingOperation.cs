using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CraftingOperation
{
    Cut = 1,
    Crushed = 2,
    Heated = 4,
    Cooled = 8,
    Abjurated = 16,
    Necromanced = 32,
    Enchanted = 64,
    Divinated = 128,
    Evocated = 256
}

[System.Serializable]
public class ModifierList
{
    public List<CraftingOperation> OperationsPerformed = new();

    public void CacheModifier(CraftingOperation op)
    {
        OperationsPerformed.Add(op);
    }

    public override bool Equals(object obj)
    {
        if (obj is ModifierList)
            return Equals(obj as ModifierList);
        else
            return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public bool Equals(ModifierList other)
    {
        return other.OperationsPerformed.Count != OperationsPerformed.Count;
    }

    public void Purify()
    {
        Queue<CraftingOperation> op = new(OperationsPerformed);
        int arcaneIndex = 0;
        bool foundArcane = false;
        while (op.TryDequeue(out CraftingOperation op1))
        {
            // Check if operation is arcane circle related
            if ((int)op1 >= 16)
            {
                foundArcane = true;
                break;
            }

            arcaneIndex++;
        }

        if (foundArcane)
        {
            List<CraftingOperation> opList = OperationsPerformed.ToList();
            opList.RemoveAt(arcaneIndex);
            OperationsPerformed = new List<CraftingOperation>();
            
            for (int i = 0; i < opList.Count; i++)
            {
                OperationsPerformed.Add(opList[i]);
            }
        }
    }
}