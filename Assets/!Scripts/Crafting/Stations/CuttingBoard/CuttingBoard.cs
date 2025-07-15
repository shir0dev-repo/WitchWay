using System;
using UnityEngine;

public class CuttingBoard : Singleton<CuttingBoard>
{
    public bool HasIngredient => _currentIngredient != null;
    private CuttableIngredient _currentIngredient = null;
    public GameObject CurrIngredientObj;
    
    public bool CanCut = false;
    public Action OnCutComplete;
    public Action OnCutCancelled;

    [SerializeField] Transform pivot;

    private void OnEnable()
    {
        GameEvents.Crafting.OnToolSelected += Enable;
        GameEvents.Crafting.OnToolDeselected += Disable;
        OnCutCancelled += CancelCutting;
    }

    private void OnDisable()
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

            if (cutPF != null)
            {
                GameObject p = Instantiate(cutPF, pivot.position, pivot.rotation);
                p.transform.SetParent(transform);
                ModifiedIngredient mod = w.ModifiedState;

                if (!(w = p.GetComponent<WorldIngredient>())) return;

                w.UpdateModifiers(mod);

                //later, this will just ask for the name of the scriptable object
                if (CursorManager.Instance != null)
                    CursorManager.Instance.ClearCursor(false);

                CurrIngredientObj = collision.gameObject;
                collision.gameObject.SetActive(false);
            }
        }
        catch
        {
            Debug.Break();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (_currentIngredient == null) return;

        if (!collision.gameObject.CompareTag("Ingredient")) return;

        if (collision.gameObject.TryGetComponent(out CuttableIngredient ing) && ing == _currentIngredient)
        {
            _currentIngredient = null;
        }

    }
    public void CancelCutting()
    {
        CurrIngredientObj.SetActive(true);
        if (CurrIngredientObj.TryGetComponent(out WorldIngredient w))
        {
            CursorManager.Instance.ClearCursor();
            w.BeginDrag();
        }

        ClearCurrentObject();
    }
    public void ClearCurrentObject()
    {
        CurrIngredientObj = null;
    }
}
