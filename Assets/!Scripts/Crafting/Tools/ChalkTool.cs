using UnityEngine;

public class ChalkTool : ToolBase
{
    protected override void OnToolDeselected()
    {
        ArcaneCircle.Instance.Enable();
    }

    protected override void OnToolSelected()
    {
        ArcaneCircle.Instance.Disable();

    }
}
