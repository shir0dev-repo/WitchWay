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
    [Space]
    [SerializeField] protected Transform RestAnchor;
    
    public Transform GetRestAnchor() => RestAnchor;
    
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
