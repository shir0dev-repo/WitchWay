using UnityEngine;

public class CauldronSpoon : ToolBase
{
    CauldronMaster cauldron;
    bool isSelected;
    float maxZ = 300, minZ = 240;

    [SerializeField] Vector3 stirRotationEulers;
    [SerializeField] Vector3 restRotationEulers;
    void Start()
    {
        cauldron = CauldronMaster.Instance;
    }
    private void Update()
    {
        if (isSelected) { ClampZRotation(); }
    }
    void ClampZRotation()
    {
        float z = Mathf.Clamp(transform.rotation.eulerAngles.z, minZ, maxZ);
        gameObject.transform.rotation = Quaternion.Euler(0, 0, z);
    }
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
