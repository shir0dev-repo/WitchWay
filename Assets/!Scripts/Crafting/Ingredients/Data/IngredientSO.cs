using UnityEngine;

[CreateAssetMenu(fileName = "New Ingredient", menuName = "Alchemy/New Ingredient")]
public class IngredientSO : SODatabase.DatabaseObject
{
    [TextArea] public string Description;

    [Space]
    [Range(0.5f, 5.0f)] public float CostMultiplier = 1.0f;
    
    [Header("Representation")]
    public Sprite Sprite;
    public GameObject WorldPrefab;

    [Header("Modification Flags")]
    public bool CanBeCut;
    public bool CanBeCrushed;
    public bool CanBeFrozen;
    public bool CanBeHeated;
    public bool CanBeMolded;
    public AlchemicalSymbol AllowedCircles;
}