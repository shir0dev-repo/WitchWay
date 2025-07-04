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
            Vector3 toIngredient = (ingredientState.transform.position - transform.position).normalized;
            float iDotP = Vector3.Dot(-transform.up, toIngredient);
            Debug.Log(iDotP);
            if (iDotP <= 0.8f) return;

            if (ingredientState.TakeDamage(_crushDamage))
            {
                if (ingredientState.TryGetComponent(out WorldIngredient ing))
                    GameEvents.Crafting.OnItemDurabilityChanged?.Invoke(ing, ingredientState.CurrentDurability);
            }
        }
    }
}