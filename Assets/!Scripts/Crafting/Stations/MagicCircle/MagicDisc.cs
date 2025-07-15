using System;
using System.Collections.Generic;
using UnityEngine;

public class MagicDisc : MonoBehaviour
{
    private List<WorldIngredient> _currentlyHeldIngredients = new();

    private List<Vector3> _ingredientAnchors = new();
    [SerializeField] private float _radius = 1.0f;
    [SerializeField] private float _verticalOffset = 0.25f;

    private void OnEnable()
    {
        GameEvents.Crafting.OnSymbolDrawn += SpellbindIngredients;
    }

    private void OnDisable()
    {
        GameEvents.Crafting.OnSymbolDrawn -= SpellbindIngredients;
    }

    private void SpellbindIngredients(AlchemicalSymbol symbol)
    {
        _currentlyHeldIngredients.ForEach(ing => ing.ModifiedState.Spellbind(symbol));
    }

    private void AddIngredient(WorldIngredient ingredient)
    {
        if (_currentlyHeldIngredients.Count >= 3) return;
        else if (_currentlyHeldIngredients.Contains(ingredient)) return;

        _currentlyHeldIngredients.Add(ingredient);
        RecalculateAnchors();
        SetIngredientPositions();

        GameEvents.Crafting.OnItemPlacedInArcaneCircle?.Invoke(ingredient);
    }

    private void RemoveIngredient(WorldIngredient ingredient)
    {
        if (_currentlyHeldIngredients.Contains(ingredient))
            _currentlyHeldIngredients.Remove(ingredient);

        RecalculateAnchors();
        SetIngredientPositions();

        GameEvents.Crafting.OnItemRemovedFromArcaneCircle?.Invoke(ingredient);
    }

    private void RecalculateAnchors()
    {
        _ingredientAnchors.Clear();

        Vector3 center = transform.position + transform.up * _verticalOffset;
        int count = _currentlyHeldIngredients.Count;
        
        if (count == 1)
        {
            _ingredientAnchors.Add(center);
            return;
        }

        float invCount = 1.0f / count;

        for (int i = 0; i < count; i++)
        {
            float cos = Mathf.Cos(2.0f * Mathf.PI * i * invCount);
            float sin = Mathf.Sin(2.0f * Mathf.PI * i * invCount);
            float x = count == 2 ? cos : sin;
            float z = count == 2 ? sin : cos;

            _ingredientAnchors.Add(center + (new Vector3(x, 0, z) * _radius));
        }
    }

    private void SetIngredientPositions()
    {
        if (_currentlyHeldIngredients.Count != _ingredientAnchors.Count)
        {
            Debug.LogWarning("Count Mismatch between anchors and ingredients!");
            return;
        }

        for (int i = 0; i < _currentlyHeldIngredients.Count; i++)
        {
            _currentlyHeldIngredients[i].transform.position = _ingredientAnchors[i];
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.TryGetComponent(out WorldIngredient ing)) return;
        if (_currentlyHeldIngredients.Count >= 3) return;

        if (CursorManager.Instance != null)
            CursorManager.Instance.ClearCursor(false);
        
        collision.collider.isTrigger = true;
        collision.rigidbody.useGravity = false;
        collision.rigidbody.linearVelocity = Vector3.zero;
        collision.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        AddIngredient(ing);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!collision.gameObject.TryGetComponent(out WorldIngredient ing)) return;
        if (CursorManager.Instance == null || CursorManager.Instance.AttachedObject != collision.transform) return;

        collision.collider.isTrigger = false;
        collision.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        collision.rigidbody.useGravity = true;
        RemoveIngredient(ing);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _radius);

        if (!Application.isPlaying) return;

        Gizmos.color = new(1, 0, 0, 0.3f);
        foreach (Vector3 anchor in _ingredientAnchors)
        {
            Gizmos.DrawSphere(anchor, 0.15f);
        }
    }
}
