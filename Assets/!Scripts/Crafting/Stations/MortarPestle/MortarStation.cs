using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class MortarStation : Singleton<MortarStation>
{
    [SerializeField] private Transform _ingredientAnchor;

    public bool HasIngredient => ingredientsInMortar.Count > 0;

    // Fail state stuff
    [SerializeField] private GameObject explosionPrefab;    // Im sure james will want to make a fun effect to play

    private List<CrushableIngredientState> ingredientsInMortar = new();
    private Dictionary<Rigidbody, RigidbodyConstraints> constraints = new();

    [SerializeField] public EventReference onToolSelected;

    private void Start()
    {
        GameEvents.Crafting.OnSuccessfullyCrushedItem += SpawnCrushedItem;
    }

    private void SpawnCrushedItem(WorldIngredient ingredient)
    {
        Vector3 spawnPosition = _ingredientAnchor.position;
        IngredientSO data = ingredient.BaseIngredient;
        ModifiedIngredient modData = ingredient.ModifiedState;
        GameObject pf = data.CrushedWorldPrefab;
        ingredientsInMortar.Clear();
        if (pf == null) return;

        GameObject ingGO = Instantiate(pf, spawnPosition, Quaternion.identity);
        ingGO.transform.SetParent(PrepBoardCraftingArea.Instance.transform);
        WorldIngredient ing = ingGO.GetComponent<WorldIngredient>();
        ing.SetIngredient(data);
        ing.UpdateModifiers(modData);

        if (ingGO.TryGetComponent(out Rigidbody rgbd))
        {
            rgbd.useGravity = false;
            constraints[rgbd] = rgbd.constraints;
            if (ingredient.TryGetComponent(out Rigidbody currRgbd) && constraints.TryGetValue(currRgbd, out RigidbodyConstraints constraint))
            {
                rgbd.constraints = constraint;
                constraints.Remove(currRgbd);
            }
        }

        Destroy(ingredient.gameObject);
    }

    private void Update()
    {
        for (int i = 0; i < ingredientsInMortar.Count; i++)
        {
            var ingredient = ingredientsInMortar[i];

            if (!ingredient.canBeCrushed)
            {
                if (ingredient.TryGetComponent(out WorldIngredient ing))
                {
                    //ing.EndDrag();
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

        if (!other.TryGetComponent(out WorldIngredient ing) || !ing.BaseIngredient.CanBeCrushed) return;

        if (CursorManager.Instance.AttachedObject == ing.transform)
            CursorManager.Instance.AssignReturnPivot(_ingredientAnchor);

        else if (other.TryGetComponent(out Rigidbody rgbd))
            other.transform.position = _ingredientAnchor.position;


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
        if (other.TryGetComponent(out Rigidbody rgbd) && constraints.TryGetValue(rgbd, out var cachedConstraints))
        {
            Debug.Log("constraints");
            rgbd.constraints = cachedConstraints;
            constraints.Remove(rgbd);
        }

        if (!other.TryGetComponent(out CrushableIngredientState state)) return;

        ingredientsInMortar.Remove(state);
        state.SetCrushable(false);

        if (!other.TryGetComponent(out WorldIngredient ing)) return;

        if (CursorManager.Instance != null && CursorManager.Instance.AttachedObject == ing.transform)
        {
            CursorManager.Instance.AssignReturnPivot(ing.transform);
            return;
        }

        GameEvents.Crafting.OnItemRemovedFromMortar?.Invoke(ing);
        if (state.CurrState != CrushState.Powder || state.CurrState != CrushState.Dust)
        {
            Debug.Log("Removed too Early!!!");
            GameEvents.Crafting.OnFailedToCrushItem?.Invoke(ing);
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

