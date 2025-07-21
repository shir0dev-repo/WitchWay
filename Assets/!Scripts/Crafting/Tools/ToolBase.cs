using UnityEngine;
using FMODUnity;

public enum ToolType : byte
{
    None = 0,
    Knife,
    Chalk,
    Pestle,
    Spoon
}

public abstract class ToolBase : MonoBehaviour
{
    public bool IsSelected { get; protected set; }
    public ToolType Type;
    [Space]
    [SerializeField] protected Transform _restAnchor;
    
    public Transform GetRestAnchor() => _restAnchor;

    [SerializeField] public EventReference onToolSelected, onToolDeselected;

    public void SelectTool()
    {
        OnToolSelected();
    }

    public void DeselectTool()
    {
        OnToolDeselected();
    }

    protected abstract void OnToolSelected();
    protected abstract void OnToolDeselected();
}
