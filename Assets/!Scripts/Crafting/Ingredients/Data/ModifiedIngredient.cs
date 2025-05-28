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
    public bool HasBeenAbjurated = false;
    public bool HasBeenNecromanced = false;
    public bool HasBeenEnchanted = false;
    public bool HasBeenDivinated = false;
    public bool HasBeenEvocated = false;

    public void Cut()
    {
        HasBeenCut = BaseIngredient.CanBeCut && true;
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

    public void Abjurate()
    {
        HasBeenAbjurated = BaseIngredient.AllowedCircles.HasFlag(AlchemicalSymbol.Abjuration) && true;
    }

    public void Necromance()
    {
        HasBeenNecromanced = BaseIngredient.AllowedCircles.HasFlag(AlchemicalSymbol.Necromancy) && true;
    }

    public void Enchant()
    {
        HasBeenEnchanted = BaseIngredient.AllowedCircles.HasFlag(AlchemicalSymbol.Enchantment) && true;
    }

    public void Divinate()
    {
        HasBeenDivinated = BaseIngredient.AllowedCircles.HasFlag(AlchemicalSymbol.Divination) && true;
    }

    public void Evocate()
    {
        HasBeenEvocated = BaseIngredient.AllowedCircles.HasFlag(AlchemicalSymbol.Evocation) && true;
    }
}
