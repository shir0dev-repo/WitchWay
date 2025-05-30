using System.Collections.Generic;
using UnityEngine;

public class RemoveLineCommand : ICommand
{
    public LineRenderer Renderer;
    public SymbolPainter Painter;

    public RemoveLineCommand(LineRenderer currentLineRenderer, SymbolPainter painter)
    {
        Renderer = currentLineRenderer;
        Painter = painter;
    }

    public void Execute()
    {
        Painter.RemoveLastLine();
        Object.Destroy(Renderer.gameObject);
        Renderer = null;
    }

    public void Undo() { }
}
