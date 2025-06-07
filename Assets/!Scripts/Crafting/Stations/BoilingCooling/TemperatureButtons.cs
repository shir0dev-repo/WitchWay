using UnityEngine;

public class TemperatureButtons : MonoBehaviour
{
    public bool isHeating;

    [SerializeField]
    float baseValue = 1f;

    private void OnMouseOver()
    {
        PotTemperature.Instance.isChangingTemp = true;
        float currTemp = PotTemperature.Instance.GetCurrentTemp();

        float toMiddle = Mathf.Clamp01(Mathf.Abs(currTemp) * 0.01f);
        float ease = Easing(toMiddle);
        float t = Mathf.Lerp(5, 0, ease);
        // toMiddle is the current temp away from zero
        // since this is called every frame and is multiplied by basevalue, t is small

        float value = baseValue * t * Time.smoothDeltaTime;
        
        if (isHeating)
        {
            PotTemperature.Instance.RaiseTemp(value);
        }
        else
        {
            PotTemperature.Instance.LowerTemp(value);
        }
    }
    private void OnMouseExit()
    {
        PotTemperature.Instance.isChangingTemp = false;
    }
    float Easing(float x)
    {
        return x < 0.5f ? 4 * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 3) / 2;
    }
}
