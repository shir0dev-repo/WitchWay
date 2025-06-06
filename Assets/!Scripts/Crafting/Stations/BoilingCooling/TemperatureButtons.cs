using UnityEngine;

public class TemperatureButtons : MonoBehaviour
{
    public bool isHeating;

    [SerializeField]
    float value = 1f;

    private void OnMouseOver()
    {
        if (isHeating) { PotTemperature.Instance.RaiseTemp(value); }
        else {  PotTemperature.Instance.LowerTemp(value); }
    }
}
