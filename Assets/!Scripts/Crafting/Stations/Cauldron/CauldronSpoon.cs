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
        gameObject.transform.rotation = Quaternion.Euler(stirRotationEulers);
    }
    protected override void OnToolDeselected()
    {
        gameObject.transform.rotation = Quaternion.Euler(restRotationEulers);
    }
}
