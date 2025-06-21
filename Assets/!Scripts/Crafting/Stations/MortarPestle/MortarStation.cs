using System;
using UnityEngine;

public class MortarStation : Singleton<MortarStation>
{
    [SerializeField] private Transform _ingredientAnchor;

    public bool HasIngredient => _currentIngredient != null;
    private bool _shouldAddIngredient = false;
    private CrushableIngredientState _currentIngredient = null;
    private RigidbodyConstraints _ingConstraintsCache = RigidbodyConstraints.None;

    protected override void Awake()
    {
        base.Awake();
        GameEvents.Crafting.OnSuccessfullyCrushedItem += SpawnCrushedItem;
        enabled = false;
    }

    private void SpawnCrushedItem(WorldIngredient ingredient)
    {
        if (_currentIngredient != null && _currentIngredient.GetComponent<WorldIngredient>() == ingredient)
        {
            GameObject crushed = ingredient.BaseIngredient.CrushedWorldPrefab;
            Vector3 pos = ingredient.transform.position;

            Destroy(_currentIngredient.gameObject);

            Instantiate(crushed, pos, Quaternion.identity);
        }
    }

    private void Update()
    {
        if (_shouldAddIngredient)
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
        }
    }

    private void OnTriggerEnter(Collider other) // trigger so it doesn't interfere with the crushing
    {
        if (this.enabled == false) return;
        if (!other.TryGetComponent(out CrushableIngredientState state)) return;

        if (other.TryGetComponent(out WorldIngredient ing) && !ing.BaseIngredient.CanBeCrushed) return;

        if (CursorManager.Instance.AttachedObject == transform)
            CursorManager.Instance.AssignReturnPivot(_ingredientAnchor);

        _shouldAddIngredient = true;
        _currentIngredient = state;
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
        //Debug.Log((int)other.GetComponent<Rigidbody>().excludeLayers);
        if (other.TryGetComponent(out CrushableIngredientState state))
        {
            if (other.TryGetComponent(out WorldIngredient ing))
                GameEvents.Crafting.OnItemRemovedFromMortar?.Invoke(ing);
            if (other.TryGetComponent(out Rigidbody rb))
            {
                rb.constraints = _ingConstraintsCache;
                _ingConstraintsCache = RigidbodyConstraints.None;
            }

            state.SetCrushable(false);
            _currentIngredient = null;
        }
    }
}

