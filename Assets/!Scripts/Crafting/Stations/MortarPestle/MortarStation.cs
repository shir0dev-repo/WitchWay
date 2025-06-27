using System;
using System.Collections.Generic;
using UnityEngine;

public class MortarStation : Singleton<MortarStation>
{
    [SerializeField] private Transform _ingredientAnchor;

    public bool HasIngredient => ingredientsInMortar.Count > 0;
    //private CrushableIngredientState _currentIngredient = null;   
    private RigidbodyConstraints _ingConstraintsCache = RigidbodyConstraints.None;

    // Fail state stuff
    [SerializeField] private GameObject explosionPrefab;    // Im sure james will want to make a fun effect to play

    private List<CrushableIngredientState> ingredientsInMortar = new();
    private Dictionary<Rigidbody, RigidbodyConstraints> constraints = new();

    private void Start()
    {
        GameEvents.Crafting.OnSuccessfullyCrushedItem += SpawnCrushedItem;
    }

    private void SpawnCrushedItem(WorldIngredient ingredient)
    {
        Vector3 spawnPosition = ingredient.transform.position;
        IngredientSO data = ingredient.BaseIngredient;
        ModifiedIngredient modData = ingredient.ModifiedState;
        GameObject pf = data.CrushedWorldPrefab;
        if (pf != null)
        {
            GameObject ingGO = Instantiate(pf, spawnPosition, Quaternion.identity);
            WorldIngredient ing = ingGO.GetComponent<WorldIngredient>();
            ing.SetIngredient(data);
            ing.UpdateModifiers(modData);
        }

        Destroy(ingredient.gameObject);
    }

    private void Update()
    {
        /*if (_shouldAddIngredient)
        {
            if (_currentIngredient.TryGetComponent(out WorldIngredient ing))
            {
                ing.EndDrag();
                GameEvents.Crafting.OnItemPlacedInMortar?.Invoke(ing);
                GameEvents.Crafting.OnItemPlacedInStation?.Invoke(ing, StationType.Mortar, _ingredientAnchor);
            }
            if (_currentIngredient.TryGetComponent(out Rigidbody rb))
            {
                _ingConstraintsCache = rb.constraints;
                rb.constraints = RigidbodyConstraints.FreezeAll;
                rb.MovePosition(_ingredientAnchor.position);
            }

            _currentIngredient.SetCrushable(true);
            _shouldAddIngredient = false;
        }*/

        for (int i = 0; i < ingredientsInMortar.Count; i++)
        {
            var ingredient = ingredientsInMortar[i];

            if (!ingredient.canBeCrushed)
            {
                if (ingredient.TryGetComponent(out WorldIngredient ing))
                {
                    ing.EndDrag();
                    GameEvents.Crafting.OnItemPlacedInMortar?.Invoke(ing);
                    GameEvents.Crafting.OnItemPlacedInStation?.Invoke(ing, StationType.Mortar, _ingredientAnchor);
                }
                if (ingredient.TryGetComponent(out Rigidbody rgbd))
                {
                    constraints[rgbd] = rgbd.constraints;
                    rgbd.constraints = RigidbodyConstraints.FreezeAll;
                    rgbd.MovePosition(_ingredientAnchor.position);
                }
                ingredient.SetCrushable(true);
            }
        }
    }

    private void OnTriggerEnter(Collider other) // trigger so it doesn't interfere with the crushing
    {
        if (this.enabled == false) return;
        if (!other.TryGetComponent(out CrushableIngredientState state)) return;

        if (other.TryGetComponent(out WorldIngredient ing) && !ing.BaseIngredient.CanBeCrushed) return;

        /* if (CursorManager.Instance.AttachedObject == transform)
             CursorManager.Instance.AssignReturnPivot(_ingredientAnchor);

         _shouldAddIngredient = true;
         _currentIngredient = state;
         */

        if (!ingredientsInMortar.Contains(state))
        {
            ingredientsInMortar.Add(state);
        }
        if (ingredientsInMortar.Count > 1)
        {
            BlowUp();
        }
    }


    private void OnTriggerStay(Collider other)
    {
        //if (_currentIngredient == null || other.gameObject != _currentIngredient.gameObject) return;

        if (Input.GetMouseButtonUp(0))
        {
            if (!other.TryGetComponent(out CrushableIngredientState state)) return;
            state.SetCrushable(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {

        /* Debug.Log((int)other.GetComponent<Rigidbody>().excludeLayers);
         if (other.TryGetComponent(out CrushableIngredientState state))
         {
             _hasIngredient = false;
             if (other.TryGetComponent(out WorldIngredient ing))
                 GameEvents.Crafting.OnItemRemovedFromMortar?.Invoke(ing);
             if (other.TryGetComponent(out Rigidbody rb))
             {
                 rb.constraints = _ingConstraintsCache;
                 _ingConstraintsCache = RigidbodyConstraints.None;
             }

             Debug.Log("mrtor");     // lol Mr Tor
             state.SetCrushable(false);
         }*/

        if (other.TryGetComponent(out CrushableIngredientState state))
        {
            ingredientsInMortar.Remove(state);

            if (other.TryGetComponent(out WorldIngredient ing))
            {
                GameEvents.Crafting.OnItemRemovedFromMortar?.Invoke(ing);
                if (state.CurrState != CrushState.Powder && state.CurrState != CrushState.Dust)
                {
                    Debug.Log("Removed too Early!!!");
                    GameEvents.Crafting.OnFailedToCrushItem?.Invoke(ing);
                }
            }
            if (other.TryGetComponent(out Rigidbody rgbd))
            {
                if (constraints.TryGetValue(rgbd, out var cachedConstraints))
                {
                    rgbd.constraints = cachedConstraints;
                    constraints.Remove(rgbd);
                }
            }
            state.SetCrushable(false);

        }
    }

    private void BlowUp()
    {
        Debug.Log("Too many ingredients, KABOOM!");

        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        }
        foreach (var ingredient in ingredientsInMortar)
        {
            if (ingredient.TryGetComponent(out WorldIngredient ing))
            {
                GameEvents.Crafting.OnFailedToCrushItem?.Invoke(ing);

                if (CursorManager.Instance.AttachedObject == ingredient.transform)
                {
                    CursorManager.Instance.ClearCursor();
                }
            }
            Destroy(ingredient.gameObject, .5f);
        }
        ingredientsInMortar.Clear();
    }
}

