using UnityEngine;

[System.Serializable]
public class ModifiedIngredient
{
    public IngredientSO BaseIngredient;
    public ModifierList ModifierList;

    [Space]
    public bool HasBeenCut = false;
    public bool HasBeenCrushed = false;
    public bool HasBeenFrozen = false;
    public bool HasBeenHeated = false;
    public AlchemicalSymbol CurrentSpell = AlchemicalSymbol.None;

    public ModifiedIngredient() : base() { }

    public ModifiedIngredient(ModifiedIngredient other)
    {
        BaseIngredient = other.BaseIngredient;
        HasBeenCut = other.HasBeenCut;
        HasBeenCrushed = other.HasBeenCrushed;
        HasBeenFrozen = other.HasBeenFrozen;
        HasBeenHeated = other.HasBeenHeated;
        CurrentSpell = other.CurrentSpell;
    }

    public GameObject GetWorldRepresentation()
    {
        if (HasBeenCut) return BaseIngredient.CutWorldPrefab;
        else if (HasBeenCrushed) return BaseIngredient.CrushedWorldPrefab;
        else return BaseIngredient.WorldPrefab;
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
    }

    public void Abjurate()
    {
        if (BaseIngredient.AllowedCircles.HasFlag(AlchemicalSymbol.Abjuration))
            CurrentSpell = AlchemicalSymbol.Abjuration;
    }

    public void Necromance()
    {
        if (BaseIngredient.AllowedCircles.HasFlag(AlchemicalSymbol.Necromancy))
            CurrentSpell = AlchemicalSymbol.Necromancy;
    }

    public void Enchant()
    {
        if (BaseIngredient.AllowedCircles.HasFlag(AlchemicalSymbol.Enchantment))
            CurrentSpell = AlchemicalSymbol.Enchantment;
    }

    public void Divinate()
    {
        if (BaseIngredient.AllowedCircles.HasFlag(AlchemicalSymbol.Divination))
            CurrentSpell = AlchemicalSymbol.Divination;
    }

    public void Evocate()
    {
        if (BaseIngredient.AllowedCircles.HasFlag(AlchemicalSymbol.Evocation))
            CurrentSpell = AlchemicalSymbol.Evocation;
    }
}
