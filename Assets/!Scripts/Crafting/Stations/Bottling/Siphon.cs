using UnityEngine;

public class Siphon : MonoBehaviour
{
    public static Siphon instance {  get; private set; }
    [SerializeField] SliderBar slider;
    // siphon requires it's own slider

    public float pressureAmount = 0;
    bool canPressButton = true;
    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canPressButton)
        {
            IncreasePressure();
        }
        if (canPressButton)
        {
            DecreasePressure();
        }

        slider.SetValue(pressureAmount);
    }

    public void IncreasePressure()
    {
        pressureAmount += 5;
        pressureAmount = ClampPressureAmount();
    }

    public void DecreasePressure()
    {
        if (pressureAmount > 75) { pressureAmount -= Time.deltaTime * 15; }
        else if (pressureAmount > 50 ) { pressureAmount -= Time.deltaTime * 10; }
        else { pressureAmount -= Time.deltaTime * 5; }

        pressureAmount = ClampPressureAmount();
    }
    float ClampPressureAmount()
    {
        return Mathf.Clamp( pressureAmount, 0, 100);
    }
    public float GetCurrentPressureAmount()
    {
        return pressureAmount;
    }
    public void ToggleButtonAbility()
    {
        canPressButton = !canPressButton;
        ResetPressure();
        // whenever the ability to increase the pressure is toggled, reset the bar's progress
    }
    public void ResetPressure()
    {
        pressureAmount = 0;
        slider.SetValue(pressureAmount);
    }
}
