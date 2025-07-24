using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PotTemperature : MonoBehaviour
{
    public static PotTemperature Instance {  get; private set; }
    public FailState_BurnCool FailState { get; private set; }
    public RandomArrowMovement arrowMovement {  get; private set; }

    public delegate void MinigameActivation();
    public static MinigameActivation StartCooking;
    public static MinigameActivation FinishCooking;
    // i have realized that you can make multiple delegates out of the same one.

    public static event Action TriggerBurning;

    [SerializeField] private Gradient _temperatureSliderGradient;
    [SerializeField] Slider_WithPointer TempSlider;
    [SerializeField] SliderBar FillValueSlider;

    float TargetTemperature => arrowMovement.ArrowValue;
    public float Temperature = 0;
    public float Progress = 0;

    private WorldIngredient _targetIngredient;
    GameObject currentIngredientInPot;

    public bool currentlyCooking { get; set; } = false;
    public bool isChangingTemp { get; set; }
    public bool amCurrentlyBurning {  get; set; }

    float TimeUntilBurn = 0;
    [SerializeField] float BurnTimerThreshold = 5f;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        
        Instance = this;
        FailState = GetComponent<FailState_BurnCool>();
        arrowMovement = GetComponent<RandomArrowMovement>();
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
        if(Temperature > 45 || Temperature < -45)
        {
            InBurningThreshold();
        }
        else { TimeUntilBurn = Mathf.Max(TimeUntilBurn - Time.deltaTime, 0); }

        if (TempSlider.isActiveAndEnabled)
        {
            if (!isChangingTemp) { EqualOutTemp(); }
            // only runs when player is not hovering on button, prevents values from fighting

            TempSlider.SetValue(Temperature);
            TempSlider.SetPointerLocation(TargetTemperature);
        }
        if (FillValueSlider.isActiveAndEnabled)
        {
            FillValueSlider.SetValue(Progress);
        }

        if (Progress >= 100)
        {
            if (_targetIngredient != null)
            {
                FinishCooking?.Invoke();
                if (TargetTemperature >= 0)
                    _targetIngredient.ModifiedState.Heat();
                else
                    _targetIngredient.ModifiedState.Freeze();

                _targetIngredient = null;
            }

        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out StateOfIngredient_BurnCool ingredient))
        {
            if (ingredient.TryGetComponent(out WorldIngredient ing))
                _targetIngredient = ing;

            currentIngredientInPot = other.gameObject;
            StartCooking?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out WorldIngredient ing) && ing == _targetIngredient)
            _targetIngredient = null;
    }

    public void SetSliderColor(float sliderValue)
    {
        float val01 = ((sliderValue / 50.0f) + 1.0f) * 0.5f;
        TempSlider.slider.targetGraphic.color = _temperatureSliderGradient.Evaluate(val01);
    }

    void StartStart()
    {
        ToggleSliders(true);

        TempSlider.SetPointerLocation(TargetTemperature);
        currentlyCooking = true;

        if (currentIngredientInPot.TryGetComponent(out WorldIngredient w))
        {
            w.enabled = false;
        }
    }
    void EndEnd()
    {
        ToggleSliders(false);
         currentlyCooking = false;

        if (currentIngredientInPot.TryGetComponent(out WorldIngredient w))
        {
            w.enabled = true;
        }
    }
    void InBurningThreshold()
    {
        TimeUntilBurn = Mathf.Min(TimeUntilBurn + Time.deltaTime, BurnTimerThreshold);

        if (TimeUntilBurn >= BurnTimerThreshold)
        {
            TriggerBurning?.Invoke();
        }
    }
    void OnBurning()
    {
        ToggleSliders(false);

        Temperature = 0;
        TimeUntilBurn = 0;
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
        Progress += 10f * Time.deltaTime;
        Progress = Mathf.Clamp(Progress, 0, 100);
    }
    public void DecreaseProgress()
    {
        Progress -= 5f * Time.deltaTime;
        Progress = Mathf.Clamp(Progress, 0, 100);
    }
    void ClampTemp()
    {
        if (Temperature > 0) { Temperature = Mathf.Clamp(Temperature, 0, 50); }
        else { Temperature = Mathf.Clamp(Temperature, -50, 0); }
    }
}
