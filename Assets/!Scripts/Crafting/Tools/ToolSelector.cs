using System;
using UnityEngine;

public class ToolSelector : Singleton<ToolSelector>
{
    public ToolBase CurrentlySelected { get; private set; }
    public ToolType CurrentType { get; private set; }

    private void Update()
    {
        if (CurrentlySelected == null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                TrySelectTool();
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(1))
            {
                DeselectTool();
            }
        }
    }

    private void TrySelectTool()
    {
        if (RaycastTool(out ToolBase tool))
        {
            CurrentlySelected = tool;
            CurrentType = tool.Type;
            CurrentlySelected.SelectTool();
            CursorManager.Instance.AttachToCursor(tool.transform, tool.GetRestAnchor());
            GameEvents.Crafting.OnToolSelected?.Invoke(CurrentType);
        }
    }

    private void DeselectTool()
    {
        CursorManager.Instance.ClearCursor();
        CurrentlySelected.DeselectTool();

        CurrentlySelected = null;

        ToolType type = CurrentType;
        CurrentType = ToolType.None;

        GameEvents.Crafting.OnToolDeselected?.Invoke(type);
    }

    private bool RaycastTool(out ToolBase tool)
    {
        tool = null;
        /*Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 1 << LayerMask.NameToLayer("UI")))
        {
            return hit.rigidbody != null && hit.rigidbody.TryGetComponent(out tool);
        }*/

        if (CursorManager.CastScreenRay(Input.mousePosition, out RaycastHit hit))
        {
            return hit.rigidbody != null && hit.rigidbody.TryGetComponent(out tool);
        }

        return false;
    }
}
