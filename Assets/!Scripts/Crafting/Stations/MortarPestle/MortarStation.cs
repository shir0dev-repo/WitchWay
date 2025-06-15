using UnityEngine;

public class MortarStation : MonoBehaviour
{
    [SerializeField] private Transform _ingredientAnchor;

    public bool HasIngredient => _hasIngredient;
    private bool _hasIngredient = false;
    private bool _shouldAddIngredient = false;
    private CrushableIngredientState _currentIngredient = null;
    private RigidbodyConstraints _ingConstraintsCache = RigidbodyConstraints.None;
    
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
        if (other.TryGetComponent(out CrushableIngredientState state))
        {
            if (other.TryGetComponent(out WorldIngredient ing) && !ing.BaseIngredient.CanBeCrushed) return;

            if (CursorManager.Instance.AttachedObject == transform)
                CursorManager.Instance.AssignReturnPivot(_ingredientAnchor);

            _shouldAddIngredient = true;
            _currentIngredient = state;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //if (_currentIngredient == null || other.gameObject != _currentIngredient.gameObject) return;
        
        if (Input.GetMouseButtonUp(0))
        {
            if (!other.TryGetComponent(out CrushableIngredientState state)) return;
            _hasIngredient = true;
            state.SetCrushable(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
        Debug.Log((int)other.GetComponent<Rigidbody>().excludeLayers);
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

            Debug.Log("mrtor");
            state.SetCrushable(false);
        }
    }
}

