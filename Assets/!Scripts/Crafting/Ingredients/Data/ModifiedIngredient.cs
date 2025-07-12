using UnityEngine;

[System.Serializable]
public class ModifiedIngredient
{
    public IngredientSO BaseIngredient;
    public ModifierList ModifierList = new ModifierList();

    [Space]
    public bool HasBeenCut = false;
    public bool HasBeenCrushed = false;
    public bool HasBeenFrozen = false;
    public bool HasBeenHeated = false;
    public AlchemicalSymbol CurrentSpell = AlchemicalSymbol.None;

    public GameObject GetWorldRepresentation()
    {
        if (HasBeenCut) return BaseIngredient.CutWorldPrefab;
        else if (HasBeenCrushed) return BaseIngredient.CrushedWorldPrefab;
        else return BaseIngredient.WorldPrefab;
    }

    public Sprite GetUIRepresentation()
    {
        if (HasBeenCrushed) return BaseIngredient.CrushedSprite;
        else if (HasBeenCut) return BaseIngredient.CutSprite;
        else return BaseIngredient.Sprite;
    }

    public void Cut()
    {
        HasBeenCut = !HasBeenCrushed && BaseIngredient.CanBeCut && true;
        ModifierList.CacheModifier(CraftingOperation.Cut);
    }

    public void Crush()
    {
        HasBeenCrushed = BaseIngredient.CanBeCrushed && true;
        ModifierList.CacheModifier(CraftingOperation.Crushed);
    }

    public void Freeze()
    {
        HasBeenFrozen = BaseIngredient.CanBeFrozen && true;
        ModifierList.CacheModifier(CraftingOperation.Cooled);
    }

    public void Heat()
    {
        HasBeenHeated = BaseIngredient.CanBeHeated && true;
        ModifierList.CacheModifier(CraftingOperation.Heated);
    }

    public void Spellbind(AlchemicalSymbol symbol)
    {
        switch (symbol)
        {
            case AlchemicalSymbol.Abjuration:
                Abjurate();
                break;
            case AlchemicalSymbol.Necromancy:
                Necromance();
                break;
            case AlchemicalSymbol.Enchantment:
                Enchant();
                break;
            case AlchemicalSymbol.Divination:
                Divinate();
                break;
            case AlchemicalSymbol.Evocation:
                Evocate();
                break;
            case AlchemicalSymbol.Purify:
                Purify();
                break;
            default:
                break;
        }
    }

    public void Purify()
    {
        CurrentSpell = AlchemicalSymbol.None;
        ModifierList.Purify();
    }

    public void Abjurate()
    {
        if (BaseIngredient.AllowedCircles.HasFlag(AlchemicalSymbol.Abjuration))
        {
            CurrentSpell = AlchemicalSymbol.Abjuration;
            ModifierList.CacheModifier(CraftingOperation.Abjurated);
        }
    }

    public void Necromance()
    {
        if (BaseIngredient.AllowedCircles.HasFlag(AlchemicalSymbol.Necromancy))
        {
            CurrentSpell = AlchemicalSymbol.Necromancy;
            ModifierList.CacheModifier(CraftingOperation.Necromanced);
        }
    }

    public void Enchant()
    {
        if (BaseIngredient.AllowedCircles.HasFlag(AlchemicalSymbol.Enchantment))
        {
            CurrentSpell = AlchemicalSymbol.Enchantment;
            ModifierList.CacheModifier(CraftingOperation.Enchanted);
        }
    }

    public void Divinate()
    {
        if (BaseIngredient.AllowedCircles.HasFlag(AlchemicalSymbol.Divination))
        {
            CurrentSpell = AlchemicalSymbol.Divination;
            ModifierList.CacheModifier(CraftingOperation.Divinated);
        }
    }

    public void Evocate()
    {
        if (BaseIngredient.AllowedCircles.HasFlag(AlchemicalSymbol.Evocation))
        {
            CurrentSpell = AlchemicalSymbol.Evocation;
            ModifierList.CacheModifier(CraftingOperation.Evocated);
        }
    }
}
