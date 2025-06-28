using UnityEngine;

public class CauldronSpoon : ToolBase
{
    CauldronMaster cauldron;
    void Start()
    {
        cauldron = CauldronMaster.Instance;
    }
    protected override void OnToolSelected()
    {
        if (StationManager.Instance.GetCurrentStation() == 3)
        {
            cauldron.ToggleMixing(!cauldron.CurrentlyMixing);
        }
    }
    protected override void OnToolDeselected()
    {
        cauldron.ToggleMixing(!cauldron.CurrentlyMixing);
    }
}
