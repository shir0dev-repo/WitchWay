using UnityEngine;

[System.Serializable]
public class ModifiedIngredient
{
    public IngredientSO BaseIngredient;
    
    [Space]
    public bool HasBeenCut = false;
    public bool HasBeenCrushed = false;
    public bool HasBeenFrozen = false;
    public bool HasBeenHeated = false;
    public bool HasBeenMolded = false;
    public AlchemicalSymbol CurrentSpell = AlchemicalSymbol.None;

    public void Cut()
    {
        HasBeenCut = !HasBeenCrushed && BaseIngredient.CanBeCut && true;
    }

    public void Crush()
    {
        HasBeenCrushed = BaseIngredient.CanBeCrushed && true;
    }

    public void Freeze()
    {
        HasBeenFrozen = BaseIngredient.CanBeFrozen && true;
    }

    public void Heat()
    {
        HasBeenHeated = BaseIngredient.CanBeHeated && true;
    }

    public void Mold()
    {
        HasBeenMolded = BaseIngredient.CanBeMolded && true;
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
