using UnityEngine;

public class MortarStation : MonoBehaviour
{
    [SerializeField] private Transform _ingredientAnchor;

    public bool HasIngredient => _hasIngredient;
    private bool _hasIngredient = false;

    private RigidbodyConstraints _ingConstraintsCache = RigidbodyConstraints.None;

    private void OnTriggerEnter(Collider other) // trigger so it doesn't interfere with the crushing
    {
        if (other.TryGetComponent(out CrushableIngredientState state))
        {
            Rigidbody rigidbody = other.gameObject.GetComponent<Rigidbody>();

            
            // changes the constraints instead of the kinematic so it still
            // generates collision stuff
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.TryGetComponent(out CrushableIngredientState state)) return;
        _hasIngredient = true;

        if (Input.GetMouseButtonUp(0))
        {
            if (other.TryGetComponent(out WorldIngredient ing))
                GameEvents.Crafting.OnItemPlacedInMortar?.Invoke(ing);
            if (other.TryGetComponent(out Rigidbody rb))
            {
                _ingConstraintsCache = rb.constraints;
                rb.constraints = RigidbodyConstraints.FreezeAll;
                rb.isKinematic = true;
                rb.MovePosition(_ingredientAnchor.position);
            }

            state.SetCrushable(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_hasIngredient) return;

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

