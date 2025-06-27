using UnityEngine;

public class KnifeTool : ToolBase
{
    [SerializeField] Vector3 cutRotationEulers;
    [SerializeField] Vector3 restRotationEulers;

    protected override void OnToolSelected()
    {
        RotateToCuttingPosition();
    }
    protected override void OnToolDeselected()
    {
        RotateToRestPosition();
    }

    private void RotateToRestPosition()
    {
        gameObject.transform.rotation = Quaternion.Euler(restRotationEulers);
    }
    private void RotateToCuttingPosition()
    { // rotates the knife by 90 degrees on the z axis
        gameObject.transform.rotation = Quaternion.Euler(cutRotationEulers);
    }
}
