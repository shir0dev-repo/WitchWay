using UnityEngine;

public class PotTemperature : MonoBehaviour
{
    public static PotTemperature Instance {  get; private set; }

    public float Temperature = 0;
    void Start()
    {
        
    }

    void Update()
    {
        
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
