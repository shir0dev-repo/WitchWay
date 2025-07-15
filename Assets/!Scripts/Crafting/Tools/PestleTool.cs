using UnityEngine;

public class PestleTool : ToolBase
{
    [SerializeField] private float _crushDamage = 5;

    private Vector3 _positionLastFrame;
    private bool _shouldUpdatePosition = false;

    protected override void OnToolSelected()
    {
        _shouldUpdatePosition = true;
    }

    protected override void OnToolDeselected()
    {
        _shouldUpdatePosition = false;
        gameObject.transform.position = _restAnchor.position;
    }

    private void FixedUpdate()
    {
        if (_shouldUpdatePosition)
        {
            _positionLastFrame = transform.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out CrushableIngredientState ingredientState)) return;

        if (!ingredientState.canBeCrushed) return;

        Vector3 vel = (transform.position - _positionLastFrame).normalized;
        Vector3 toIngredient = (ingredientState.transform.position - transform.position).normalized;
        float iDotP = Vector3.Dot(-transform.up, vel);
        Debug.Log(iDotP);

        if (iDotP <= 0.8f) return;

        if (ingredientState.TakeDamage(_crushDamage))
        {
            if (ingredientState.TryGetComponent(out WorldIngredient ing))
                GameEvents.Crafting.OnItemDurabilityChanged?.Invoke(ing, ingredientState.CurrentDurability);
        }
    }
}