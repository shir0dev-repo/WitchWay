using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FMODUnity;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(SymbolPainter))]
public class ArcaneCircle : Singleton<ArcaneCircle>
{
    [Serializable] public struct GestureSymbolPair
    {
        public string GestureName;
        public Sprite Sprite;
        public AlchemicalSymbol Symbol;
        [Range(0, 1)] public float AccuracyThreshold;
    }

    [Header("Scene References")]
    [SerializeField] private SymbolPainter _painter;
    [SerializeField] private GameObject _validateBtn;
    [SerializeField] private Image _flashedSymbol;
    [Space]
    [SerializeField] private StationAreaType _areaType;
    [Space]
    [SerializeField] private List<GestureSymbolPair> _symbols;

    private Stack<RemoveLineCommand> _savedLines = new();

    [Header("Audio")]
    [SerializeField] private EventReference drawValidSound;

    public void Enable(ToolType type)
    {
        if (type == ToolType.Chalk)
        {
            StationManager.Instance.ToggleDrag(false);
            _painter.enabled = true;
        }
    }

    public void Disable(ToolType type)
    {
        if (type == ToolType.Chalk)
        {
            StationManager.Instance.ToggleDrag(true);
            _painter.enabled = false;
        }
    }

    private void SetupStation(int stationID)
    {
        _validateBtn.SetActive(stationID == _areaType.GetStationAreaID());
        _painter.Clear();
    }

    private void OnEnable()
    {
        GameEvents.Crafting.OnToolSelected += Enable;
        GameEvents.Crafting.OnToolDeselected += Disable;
        GameEvents.Crafting.OnStationChanged += SetupStation;

        SymbolPainter.OnLineDrawn += FinishLineDraw;
        SymbolPainter.OnGestureCompleted += ValidateSymbol;
    }

    private void OnDisable()
    {
        GameEvents.Crafting.OnToolSelected -= Enable;
        GameEvents.Crafting.OnToolDeselected -= Disable;
        GameEvents.Crafting.OnStationChanged -= SetupStation;

        SymbolPainter.OnLineDrawn -= FinishLineDraw;
        SymbolPainter.OnGestureCompleted -= ValidateSymbol;
    }

    private void Start()
    {
        _validateBtn.SetActive(false);
        _painter.enabled = false;
    }

    private void ValidateSymbol(string symbolName, float accuracy)
    {
        _savedLines.Clear();

        AlchemicalSymbol result = AlchemicalSymbol.None;
        bool valid = _symbols.Any(gesturePair =>
        {
            if (gesturePair.GestureName == symbolName && accuracy >= gesturePair.AccuracyThreshold)
            {
                result = gesturePair.Symbol;
                SoundManager.Instance.PlayOneShot(drawValidSound, Camera.main.transform.position);
                return true;
            }

            return false;
        });

        if (valid)
        {
            Debug.Log($"{symbolName}: {accuracy:F2}%");
            _flashedSymbol.sprite = _symbols.FirstOrDefault(s =>  s.GestureName == symbolName).Sprite;
            _flashedSymbol.color = Color.white;
            DOTween.To(() => _flashedSymbol.color, (c) => _flashedSymbol.color = c, Color.clear, 1.5f).SetEase(Ease.OutCubic);
            GameEvents.Crafting.OnSymbolDrawn?.Invoke(result);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_savedLines.TryPop(out RemoveLineCommand undo))
            {
                undo.Execute();
            }
        }
    }

    public void BeginDraw()
    {
        
    }

    public void FinishLineDraw()
    {
        _savedLines.Push(new RemoveLineCommand(_painter.CurrentGestureRenderer, _painter));
    }
}
