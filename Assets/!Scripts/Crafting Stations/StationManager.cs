using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class StationManager : MonoBehaviour
{
    public const int _STATION_COUNT = 4;
    public static StationManager Instance { get; private set; }

    [Header("Controls")]
    [SerializeField] private InputAction _changeStationAction;

    [Header("Cutting Board")]
    [SerializeField] private CuttingBoard _cuttingBoard;
    [SerializeField] private Transform _cuttingBoardTransform;

    [Header("Mortar & Pestle")]
    [SerializeField] private Pestle _pestle;
    [SerializeField] private Transform _mortarPestleTransform;

    [Header("Magic Circle")]
    [SerializeField] private SymbolPainter _magicCircle;
    [SerializeField] private Transform _magicCircleTransform;

    [Header("Cauldron")]
    [SerializeField] private Cauldron _cauldron;
    [SerializeField] private Transform _cauldronArea;

    private int _currentTransformIndex = 0;
    
    [System.Serializable]
    public class StationChangedEvent : UnityEvent<int> { }

    public StationChangedEvent OnStationChanged = new StationChangedEvent();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void OnEnable()
    {
        _changeStationAction.started += MoveToStation;
        _changeStationAction.Enable();
    }

    private void OnDisable()
    {
        _changeStationAction.started -= MoveToStation;
        _changeStationAction.Disable();
    }

    private void Update()
    {
        
    }

    public void GoPreviousStation()
    {
        _currentTransformIndex--;

        if (_currentTransformIndex < 0)
            _currentTransformIndex = _STATION_COUNT - 1;

        SwapStation(_currentTransformIndex);
    }

    public void GoNextStation()
    {
        _currentTransformIndex = (_currentTransformIndex + 1) % _STATION_COUNT;
        SwapStation(_currentTransformIndex);
    }


    public void SwapStation(int targetStation)
    {
        Vector3 targetPos = (targetStation) switch
        {
            0 => _cuttingBoardTransform.position,  // Cutting Board
            1 => _mortarPestleTransform.position,  // Mortar and Pestle
            2 => _magicCircleTransform.position,   // Magic Circle
            3 => _cauldronArea.position,           // Cauldron
            _ => Vector3.zero
        };

        CameraManager.Instance.MoveToPosition(targetPos);
        _currentTransformIndex = targetStation;
        OnStationChanged.Invoke(targetStation);
    }

    private void MoveToStation(InputAction.CallbackContext context)
    {
        float input = context.ReadValue<float>();

        if (input < 0)
            GoPreviousStation();
        else if (input > 0)
            GoNextStation();
    }
}
