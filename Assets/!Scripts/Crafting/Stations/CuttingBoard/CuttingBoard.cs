using System;
using FMODUnity;
using UnityEngine;

public class CuttingBoard : CraftingStation<CuttingBoard>
{
    public bool HasIngredient => _currentIngredient != null;
    public CuttableIngredient CurrentIngredient => _currentIngredient;
    private CuttableIngredient _currentIngredient = null;

    public bool CanCut = false;
    public Action OnCutComplete;
    public Action OnCutCancelled;

    [SerializeField] Transform pivot;

    [Header("Sounds")]
    public EventReference onKnifeCutSound, onKnifeFailSound;

    protected override void OnEnable()
    {
        GameEvents.Crafting.OnToolSelected += Enable;
        GameEvents.Crafting.OnToolDeselected += Disable;
        GameEvents.Crafting.OnObjectAttachedToCursor += ClearIngredient;
        GameEvents.Crafting.OnSuccessfullyCutItem += _ => _currentIngredient = null;
        //OnCutCancelled += RevertCurrentIngredient;
    }

    protected override void OnDisable()
    {
        GameEvents.Crafting.OnToolSelected -= Enable;
        GameEvents.Crafting.OnToolDeselected -= Disable;
    }

    private void Enable(ToolType type)
    {
        if (type == ToolType.Knife)
            CanCut = true;
    }

    private void Disable(ToolType type)
    {
        if (type == ToolType.Knife)
            CanCut = false;
    }

    private void ClearIngredient(IFollowCursor cursor)
    {
        if (cursor is not WorldIngredient cuttable) return;
        if (cuttable.GetComponent<CuttableIngredient>() == _currentIngredient)
            _currentIngredient = null;
    }

    public void ChangeCuttingAbility()
    { // changed this into function so it can be called in other scripts
        CanCut = !CanCut;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (_currentIngredient != null) return;

        try
        {
            if (!collision.gameObject.TryGetComponent(out WorldIngredient w)) return;

            IngredientSO ingredient = w.BaseIngredient;
            if (ingredient.CanBeCut == false) return;
            GameObject cutPF = ingredient.CutWorldPrefab;

            if (cutPF == null) return;

            GameObject p = Instantiate(cutPF, pivot.position, pivot.rotation);
            if (!p.TryGetComponent(out _currentIngredient))
            {
                Destroy(p);
                return;
            }

            p.transform.SetParent(transform);
            ModifiedIngredient mod = w.ModifiedState;

            if (!(w = p.GetComponent<WorldIngredient>())) return;

            w.UpdateModifiers(mod);

            //later, this will just ask for the name of the scriptable object
            if (CursorManager.Instance != null)
                CursorManager.Instance.ClearCursor(false);

            Destroy(collision.gameObject);
            
            GameEvents.Crafting.OnItemPlacedOnCuttingBoard?.Invoke(w);
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
            Debug.Break();
        }
    }

    public void RevertCurrentIngredient()
    {
        if (_currentIngredient == null) return;
        Debug.Log("reverting");
        if (!_currentIngredient.TryGetComponent(out WorldIngredient wIng)) return;

        ModifiedIngredient modifiedState = wIng.ModifiedState;
        GameObject ingGO = modifiedState.GetWorldRepresentation();
        GameObject newIng = Instantiate(ingGO, _currentIngredient.transform.position, Quaternion.identity);

        if (newIng.TryGetComponent(out WorldIngredient wIng2))
        {
            wIng2.UpdateModifiers(modifiedState);
        }
        CursorManager.Instance.ClearCursor(false);
        CursorManager.Instance.AttachToCursor(wIng2, newIng.transform);
        

        Destroy(_currentIngredient.gameObject);
        _currentIngredient = null;
    }

    protected override bool CanAddIngredient(WorldIngredient ingredient)
    {
        return _currentIngredient == null;
    }

    protected override bool WasProcessCompleted()
    {
        return false;
    }

    protected override void ApplyIngredientModifiers()
    {
        if (_currentIngredient == null) return;
        _currentIngredient.GetComponent<WorldIngredient>().ModifiedState.Cut();
    }

    public override void AddIngredient(WorldIngredient ingredient)
    {
        if (!ingredient.BaseIngredient.CanBeCut) return;
        if (ingredient.ModifiedState.HasBeenCut) return;
        if (!ingredient.TryGetComponent(out CuttableIngredient cuttable)) return;

        _currentIngredient = cuttable;
        GameEvents.Crafting.OnItemPlacedOnCuttingBoard?.Invoke(ingredient);
    }

    public override void RemoveIngredient(bool shouldDestroy = false)
    {
        if (_currentIngredient == null) return;

        if (!WasProcessCompleted())
        {
            RevertCurrentIngredient();
            return;
        }
    }
}
