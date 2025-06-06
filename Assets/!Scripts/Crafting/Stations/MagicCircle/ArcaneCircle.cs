using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SymbolPainter))]
public class ArcaneCircle : Singleton<ArcaneCircle>
{
    [System.Serializable]
    public struct GestureSymbolPair
    {
        public string GestureName;
        public AlchemicalSymbol Symbol;
        [Range(0, 1)] public float AccuracyThreshold;
    }

    public static Action<AlchemicalSymbol> OnSymbolPainted;

    [SerializeField] private SymbolPainter _painter;
    [Space]
    [SerializeField] private List<GestureSymbolPair> _symbols;

    private Stack<RemoveLineCommand> _savedLines = new();

    public void Enable()
    {
        _painter.enabled = true;
    }

    public void Disable()
    {
        _painter.Clear();
        _painter.enabled = false;
    }

    private void OnEnable()
    {
        SymbolPainter.OnLineDrawn += FinishDraw;
        SymbolPainter.OnGestureCompleted += ValidateSymbol;
    }

    private void OnDisable()
    {
        SymbolPainter.OnLineDrawn -= FinishDraw;
        SymbolPainter.OnGestureCompleted -= ValidateSymbol;
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
                return true;
            }

            return false;
        });

        if (valid)
        {
            Debug.Log($"{symbolName}: {accuracy:F2}%");
            OnSymbolPainted?.Invoke(result);
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

    public void FinishDraw()
    {
        _savedLines.Push(new RemoveLineCommand(_painter.CurrentGestureRenderer, _painter));
    }
}
