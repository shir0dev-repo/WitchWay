using System;
using UnityEngine;

public class PotTemperature : MonoBehaviour
{
    public static PotTemperature Instance {  get; private set; }

    public delegate void StartMinigame();
    public static StartMinigame StartCooking;

    public delegate void FinishMinigame();
    public static FinishMinigame FinishCooking;

    public static event Action TriggerBurning;

    [SerializeField] Slider_WithPointer TempSlider; 
    [SerializeField] SliderBar FillValueSlider;

    public float TargetTemperature;
    public float Temperature = 0;
    public float Progress = 0;
    public bool isChangingTemp = false;
    public bool amCurrentlyBurning = false;
    void Awake()
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
    private void OnEnable()
    {
        StartCooking += StartStart;
        FinishCooking += EndEnd;
        TriggerBurning += OnBurning;
    }
    private void OnDisable()
    {
        StartCooking = null;
        FinishCooking = null;
        TriggerBurning = null;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TriggerBurning?.Invoke();
        }
        // for debugging purposes, i'll add the timer later

        if (TempSlider.isActiveAndEnabled)
        {
            if (!isChangingTemp) { EqualOutTemp(); }
            // only runs when player is not hovering on button, prevents values from fighting

            TempSlider.SetValue(Temperature);
        }
        if (FillValueSlider.isActiveAndEnabled)
        {
            FillValueSlider.SetValue(Progress);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out StateOfIngredient_BurnCool ingredient))
        {
            StartCooking?.Invoke();
        }
    }
    void StartStart()
    {
        ToggleSliders(true);

        TempSlider.SetPointerLocation(TargetTemperature);
    }
    void EndEnd()
    {
        ToggleSliders(false);
    }
    void OnBurning()
    {
        ToggleSliders(false);

        Temperature = 0;
        amCurrentlyBurning = true;
    }
    public void ToggleSliders(bool value)
    {
        TempSlider.gameObject.SetActive(value);
        FillValueSlider.gameObject.SetActive(value);
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
    void EqualOutTemp()
    {
        float toMiddle = Mathf.Clamp01(Mathf.Abs(Temperature) * 0.01f);
        float value = Mathf.Lerp(5, 0, toMiddle);
        // toMiddle is the temp's percentage away from zero, no matter if its pos or neg
        // since this is called every frame, make value small
        
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
    public void IncreaseProgress()
    {
        Progress += 7.5f * Time.deltaTime;
        Progress = Mathf.Clamp(Progress, 0, 100);
    }
    public void DecreaseProgress()
    {
        Progress -= 5 * Time.deltaTime;
        Progress = Mathf.Clamp(Progress, 0, 100);
    }
    void ClampTemp()
    {
        if (Temperature > 0) { Temperature = Mathf.Clamp(Temperature, 0, 50); }
        else { Temperature = Mathf.Clamp(Temperature, -50, 0); }
    }
}
