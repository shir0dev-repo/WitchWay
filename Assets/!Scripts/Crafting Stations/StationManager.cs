using UnityEngine;

public class StationManager : MonoBehaviour
{
    public static StationManager Instance { get; private set; }

    [Header("Stations")]
    [Header("Cauldron")]
    [SerializeField] private Cauldron _cauldron;
    [SerializeField] private Transform _cauldronArea;
    
    [Header("Cutting Board")]
    [SerializeField] private CuttingBoard _cuttingBoard;
    [SerializeField] private Transform _cuttingBoardTransform;

    [Header("Magic Circle")]
    [SerializeField] private SymbolPainter _magicCircle;
    [SerializeField] private Transform _magicCircleTransform;
    
    [Header("Mortar & Pestle")]
    [SerializeField] private Pestle _pestle;
    [SerializeField] private Transform _mortarPestleTransform;

    private int _currentTransformIndex = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
}
