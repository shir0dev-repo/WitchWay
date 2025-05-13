using UnityEngine;

public enum SortingMode { Size, Gradient }
public enum GroupingMode { Color, Plant }

[System.Serializable]
public abstract class RecipePuzzle 
{
    public abstract SortingMode SortingMode { get; }
    [SerializeField] protected float _distanceThreshold = 5.0f;
    public abstract bool IsSorted();
    [SerializeField] protected Transform[] _ingredientTransforms;
}
