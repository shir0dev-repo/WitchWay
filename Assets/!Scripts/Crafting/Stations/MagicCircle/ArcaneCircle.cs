using System.Collections.Generic;
using UnityEngine;

public class ArcaneCircle : MonoBehaviour
{
    [SerializeField] private SymbolPainter _painter;

    private Stack<RemoveLineCommand> _savedLines = new();

    private void Start()
    {
        SymbolPainter.OnLineDrawn += FinishDraw;
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
