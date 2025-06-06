using UnityEngine;

public enum ToolType : byte
{
    None = 0,
    Knife,
    Chalk,
    Pestle
}

public abstract class ToolBase : MonoBehaviour
{
    public bool IsSelected { get; protected set; }
    public ToolType Type;


    public void SelectTool()
    {
        OnToolSelected();
        GameEvents.Crafting.OnToolSelected?.Invoke(Type);
    }

    public void DeselectTool()
    {
        OnToolDeselected();
        GameEvents.Crafting.OnToolDeselected?.Invoke(Type);
    }

    protected abstract void OnToolSelected();
    protected abstract void OnToolDeselected();
}
