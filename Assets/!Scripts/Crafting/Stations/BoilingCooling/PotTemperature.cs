using UnityEngine;

public class PotTemperature : MonoBehaviour
{
    public static PotTemperature Instance {  get; private set; }
    [SerializeField] SliderBar slider;

    public float Temperature = 0;
    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    void Update()
    {
        EqualOutTemp();
        slider.SetValue(Temperature);
    }
    public void RaiseTemp(float amount)
    {
        Temperature += amount;
    }
    public void LowerTemp(float amount)
    {
        Temperature -= amount;
    }
    public float GetCurrentTemp()
    {
        return Temperature;
    }
    void EqualOutTemp()
    {
        float toMiddle = Mathf.Clamp01(Mathf.Abs(Temperature) * 0.01f);
        float value = Mathf.Lerp(50, 0, toMiddle);
        // toMiddle is the temp's percentage away from zero, no matter if its pos or neg
        // value is the easing towards zero
        
        if (Temperature > 0)
        {
            Temperature -= value * Time.deltaTime;
            if (Temperature < 0.001f) Temperature = 0;
            // snapping to zero to prevent infinite calculations
        }
        else if (Temperature < 0)
        {
            Temperature += value * Time.deltaTime;
            if (Temperature > 0.001f) Temperature = 0;
        }
        else { Temperature = 0; }
        
        ClampTemp();
    }
    void ClampTemp()
    {
        if (Temperature > 0) { Temperature = Mathf.Clamp(Temperature, 0, 50); }
        else { Temperature = Mathf.Clamp(Temperature, -50, 0); }
    }
}
