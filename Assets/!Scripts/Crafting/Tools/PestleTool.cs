using UnityEngine;

public class PestleTool : ToolBase
{
    [SerializeField] private float _crushDamage = 5;

    protected override void OnToolSelected()
    {

    }

    protected override void OnToolDeselected()
    {
        ReturnToPosition();
    }

    void ReturnToPosition()
    {
        gameObject.transform.position = _restAnchor.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CrushableIngredientState ingredientState))
        {
            if (ingredientState.TakeDamage(_crushDamage))
            {
                if (ingredientState.TryGetComponent(out WorldIngredient ing))
                    GameEvents.Crafting.OnItemDurabilityChanged?.Invoke(ing, ingredientState.CurrentDurability);
            }
        }
    }
}