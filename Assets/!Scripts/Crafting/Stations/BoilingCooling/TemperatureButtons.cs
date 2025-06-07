using UnityEngine;

public class TemperatureButtons : MonoBehaviour
{
    public bool isHeating;

    [SerializeField]
    float baseValue = 5f;

    private void OnMouseOver()
    {
        float currTemp = PotTemperature.Instance.GetCurrentTemp();

        float toMiddle = Mathf.Clamp01(Mathf.Abs(currTemp)/50);
        float ease = Easing(toMiddle);
        // toMiddle is the current temp away from zero

        float value = baseValue * ease * Time.smoothDeltaTime;
        
        if (isHeating)
        {
            PotTemperature.Instance.RaiseTemp(value);
        }
        else
        {
            PotTemperature.Instance.LowerTemp(value);
        }
        Debug.Log($"Temperature: {currTemp} \ntoMiddle: {toMiddle}\neasing: {ease}n\value: {value}");
    }
    float Easing(float x)
    {
        return x * x * x * x * x;
    }
}
