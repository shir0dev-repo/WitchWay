using UnityEngine;

public class CauldronSpoon : ToolBase
{
    CauldronMaster cauldron;

    [SerializeField] Vector3 stirRotationEulers;
    [SerializeField] Vector3 restRotationEulers;
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

        gameObject.transform.rotation = Quaternion.Euler(stirRotationEulers);
    }
    protected override void OnToolDeselected()
    {
        cauldron.ToggleMixing(!cauldron.CurrentlyMixing);
        gameObject.transform.rotation = Quaternion.Euler(restRotationEulers);
    }
}
