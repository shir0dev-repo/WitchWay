using UnityEngine;

public class CauldronSpoon : ToolBase
{
    [SerializeField] Vector3 stirRotationEulers;
    [SerializeField] Vector3 restRotationEulers;

    protected override void OnToolSelected()
    {
        gameObject.transform.rotation = Quaternion.Euler(stirRotationEulers);
        isSelected = true;
    }
    protected override void OnToolDeselected()
    {
        gameObject.transform.rotation = Quaternion.Euler(restRotationEulers);
        isSelected = false;
    }
}
