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
        slider.SetValue(Temperature);
    }
    public void RaiseTemp(float amount)
    {
        Temperature += amount;
        ClampTemp();
    }
    public void LowerTemp(float amount)
    {
        Temperature -= amount;
        ClampTemp();
    }
    public float GetCurrentTemp()
    {
        return Temperature;
    }
    void ClampTemp()
    {
        Temperature = Mathf.Clamp(Temperature, -50, 50);
    }
}
