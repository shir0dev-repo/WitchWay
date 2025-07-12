using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "New Ingredient", menuName = "Alchemy/New Ingredient")]
public class IngredientSO : SODatabase.DatabaseObject
{
    [TextArea] public string Description;

    [Space]
    [Range(0.5f, 5.0f)] public float CostMultiplier = 1.0f;

    [Header("UI Representation")]
    public Sprite Sprite;
    public Sprite CutSprite;
    public Sprite CrushedSprite;

    [Header("World Representation")]
    public GameObject WorldPrefab;
    public GameObject CutWorldPrefab;
    public GameObject CrushedWorldPrefab;

    [Header("Audio Representation")]
    public EventReference OnCutAudioClip;
    public EventReference OnCrushAudioClip;
    public EventReference OnPickupAudioClip;
    public EventReference OnPutDownAudioClip;

    [Header("Modification Flags")]
    public bool CanBeCut;
    public bool CanBeCrushed;
    public bool CanBeFrozen;
    public bool CanBeHeated;
    public bool CanBeMolded;
    public AlchemicalSymbol AllowedCircles;

    public bool CanBeUsedAtStation(StationType station)
    {
        switch (station)
        {
            case StationType.CuttingBoard: return CanBeCut;
            case StationType.Mortar: return CanBeCrushed;
            case StationType.TemperaturePot: return CanBeFrozen || CanBeHeated;
            
            //add more when more stations
            default: return true;
        }
    }
}