using UnityEngine;

public class TemperatureButtons : MonoBehaviour
{
    PotTemperature pot;
    public bool isHeating;

    [SerializeField]
    float baseValue = 1f;
    private void Start()
    {
        pot = PotTemperature.Instance;
    }
    private void OnMouseOver()
    {
        pot.isChangingTemp = true;
        float currTemp = pot.GetCurrentTemp();

        float toMiddle = Mathf.Clamp01(Mathf.Abs(currTemp) * 0.01f);
        float ease = Easing(toMiddle);
        float t = Mathf.Lerp(5, 0, ease);
        // toMiddle is the current temp away from zero
        // since this is called every frame and is multiplied by basevalue, t is small

        float value = baseValue * t * Time.smoothDeltaTime;
        
        if (isHeating)
        {
            pot.RaiseTemp(value);
        }
        else
        {
            pot.LowerTemp(value);
        }
    }
    private void OnMouseExit()
    {
        pot.isChangingTemp = false;
    }
    float Easing(float x)
    {
        return x < 0.5f ? 4 * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 3) / 2;
    }
}
