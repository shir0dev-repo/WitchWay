using System.Collections.Generic;
using UnityEngine;

public abstract class CraftingStation<T> : Singleton<T> where T : MonoBehaviour
{
    [SerializeField] protected Bounds _workingArea;

    protected readonly List<WorldIngredient> _currentIngredients = new();

    protected virtual void OnEnable()
    {
        GameEvents.Crafting.OnObjectRemovedFromCursor += OnCursorReleasedItem;
        GameEvents.Crafting.OnObjectAttachedToCursor += OnCursorGrabbedItem;
    }

    protected virtual void OnDisable()
    {
        GameEvents.Crafting.OnObjectRemovedFromCursor -= OnCursorReleasedItem;
        GameEvents.Crafting.OnObjectAttachedToCursor -= OnCursorGrabbedItem;
    }

    private void OnCursorGrabbedItem(IFollowCursor cursor)
    {
        if (cursor == null) return;
        if (cursor is not WorldIngredient ingredient) return;
        if (!_currentIngredients.Contains(ingredient)) return;

        _currentIngredients.Remove(ingredient);
    }

    private void OnCursorReleasedItem(IFollowCursor cursor)
    {
        if (cursor == null) return;
        if (cursor is not WorldIngredient ingredient) return;

        if (!IsInsideWorkArea(ingredient.transform.position)) return;

        if (CanAddIngredient(ingredient))
            AddIngredient(ingredient);
    }

    protected abstract bool CanAddIngredient(WorldIngredient ingredient);
    protected abstract bool WasProcessCompleted();
    protected abstract void ApplyIngredientModifiers();
    public abstract void AddIngredient(WorldIngredient ingredient);
    public abstract void RemoveIngredient(bool shouldDestroy = false);
    

    protected bool IsInsideWorkArea(Vector3 position)
    {
        return _workingArea.Contains(position);
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.35f);

        Gizmos.DrawCube(_workingArea.center, _workingArea.size);
    }
}
