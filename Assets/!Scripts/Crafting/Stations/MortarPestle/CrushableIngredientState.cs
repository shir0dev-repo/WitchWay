using System;
using UnityEngine;

public enum CrushState
{
    Whole,
    Chunky,
    Crumbly,
    Powder,
    Dust
}

public class CrushableIngredientState : MonoBehaviour
{
    public CrushState CurrState;

    [SerializeField] private float _maxDurability = 100;
    public float CurrentDurability { get; private set; } = 100;

    public bool canBeCrushed = false;

    private bool _isCrushableIngredient;
    
    private void Start()
    {
        _isCrushableIngredient = CheckCrushability();
        CurrState = CrushState.Whole;
    }

    private bool CheckCrushability()
    {
        if (TryGetComponent(out WorldIngredient ing) && ing.BaseIngredient != null)
        {
            return ing.BaseIngredient.CanBeCrushed;
        }

        return false;
    }

    public bool TakeDamage(float dmg)
    {
        if (CurrentDurability <= 0) { return false; }
        CurrentDurability -= dmg;
        ChangeState();
        return true;
    }

    private void ChangeState()
    {
        Mathf.Clamp(CurrentDurability, 0, 100);

        if (CurrentDurability > 70)
        {
            CurrState = CrushState.Chunky;
        }
        else if (CurrentDurability > 40)
        {
            CurrState = CrushState.Crumbly;
        }
        else if (CurrentDurability > 10)
        {
            CurrState = CrushState.Powder;
        }
        else
        {
            CurrState = CrushState.Dust;
            GetComponent<WorldIngredient>().ModifiedState.Crush();
            GameEvents.Crafting.OnSuccessfullyCrushedItem?.Invoke(GetComponent<WorldIngredient>());
        }

        Debug.Log("Ingredient is currently: " + CurrState.ToString() + "\n"
            + "Ingredient's Durability: " + CurrentDurability.ToString());
    }

    public void SetCrushable(bool toggle)
    {
        // reset durability when placed in mortar
        Debug.Log("Toggling crush: " + toggle.ToString());
        if (toggle) CurrentDurability = _maxDurability;
        canBeCrushed = toggle;
    }
}
