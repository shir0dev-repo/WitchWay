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
        
    }

    private void DeselectTool()
    {
        CurrentlySelected.DeselectTool();

        CurrentlySelected = null;
        CurrentType = ToolType.None;
    }
}
