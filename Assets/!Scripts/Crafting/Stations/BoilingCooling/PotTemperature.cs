using NUnit.Framework;
using System;
using UnityEngine;
using FMODUnity;

public class PotTemperature : MonoBehaviour
{
    public static PotTemperature Instance {  get; private set; }
    public FailState_BurnCool FailState { get; private set; }
    public RandomArrowMovement ArrowMovement {  get; private set; }

    public delegate void MinigameActivation();
    public static MinigameActivation StartCooking;
    public static MinigameActivation FinishCooking;
    // i have realized that you can make multiple delegates out of the same one.

    public static event Action TriggerBurning;

    [Header("Mechanic Settings")]
    [SerializeField] private float _progressIncrement = 5.0f;
    [SerializeField] private float _progressDecrement = 4.0f;
    [SerializeField] float BurnTimerThreshold = 5f;

    [Header("References", order = 1)]
    [SerializeField] Slider_TwoPointers TempSlider;
    [SerializeField] SliderBar FillValueSlider;

    [Header("Visuals")]
    [SerializeField] private Gradient _temperatureSliderGradient;

    float TargetTemperature => ArrowMovement.TrueArrow;
    public float Temperature { get; private set; } = 0;
    public float Progress { get; private set; } = 0;

    public WorldIngredient GetWorldIngredient() { return _targetIngredient; }
    private WorldIngredient _targetIngredient;
    GameObject currentIngredientInPot;

    public bool CurrentlyCooking { get; set; } = false;
    public bool IsChangingTemp { get; set; }
    public bool IsCurrentlyBurning {  get; set; }

    float TimeUntilBurn = 0;

    [Header("Audio")]
    public EventReference heatingSound;
    public EventReference coolingSound;
    public EventReference constantTempSound;
    public EventReference successSound;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        
        Instance = this;
        FailState = GetComponent<FailState_BurnCool>();
        ArrowMovement = GetComponent<RandomArrowMovement>();
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
            InBurningThreshold();
        else 
            TimeUntilBurn = Mathf.Max(TimeUntilBurn - Time.deltaTime, 0);

        if (TempSlider.isActiveAndEnabled)
        {
            // only runs when player is not hovering on button, prevents values from fighting
            if (!IsChangingTemp) 
                EqualOutTemp();

            TempSlider.SetValue(Temperature);
            SetSliderPointers();

            if(!SoundManager.Instance.IsLooping("ConstantTempSound"))
            SoundManager.Instance.PlayLoop("ConstantTempSound", constantTempSound, transform.position);

            if (SoundManager.Instance.IsLooping("ConstantTempSound"))
            {
                float normalizedTemp = Mathf.InverseLerp(-50f, 50f, Temperature);
                SoundManager.Instance.SetParameterByName("ConstantTempSound", "temperature", normalizedTemp);
            }
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
        SetSliderPointers();

        CurrentlyCooking = true;

        if (currentIngredientInPot.TryGetComponent(out WorldIngredient w))
        {
            ArrowMovement.CanBeCooled = w.BaseIngredient.CanBeFrozen;
            ArrowMovement.CanBeHeated = w.BaseIngredient.CanBeHeated;
            
            w.enabled = false;
        }
    }
    void EndEnd()
    {
        TempSlider.SetValue(0); FillValueSlider.SetValue(0);
        Temperature = 0; Progress = 0;
        TimeUntilBurn = 0;

        CurrentlyCooking = false;
        IsChangingTemp = false;
        IsCurrentlyBurning = false;
        
        ToggleSliders(false);

        if (currentIngredientInPot.TryGetComponent(out WorldIngredient w))
        {
            w.enabled = true;
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.StopLoop("ConstantTempSound");
            SoundManager.Instance.PlayOneShot(successSound, transform.position);
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
        IsCurrentlyBurning = true;

        if (SoundManager.Instance != null)
            SoundManager.Instance.StopLoop("ConstantTempSound");
    }
    public void ToggleSliders(bool value)
    {
        TempSlider.gameObject.SetActive(value);
        FillValueSlider.gameObject.SetActive(value);

        if (value)
        {
            if (_targetIngredient.BaseIngredient.CanBeHeated) { TempSlider.pointer1.SetActive(true); }
            else { TempSlider.pointer1.SetActive(false); }

            if (_targetIngredient.BaseIngredient.CanBeFrozen) { TempSlider.pointer2.SetActive(true); }
            else { TempSlider.pointer2.SetActive(false); }
        }
        else { return; }
    }
    void SetSliderPointers()
    {
        if (_targetIngredient.BaseIngredient.CanBeHeated)
        {
            TempSlider.SetPointerLocation(ArrowMovement.HeatArrow, TempSlider.pointer1);
        }

        if (_targetIngredient.BaseIngredient.CanBeFrozen)
        {
            TempSlider.SetPointerLocation(ArrowMovement.CoolArrow, TempSlider.pointer2);
        }
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
        Progress += _progressIncrement * Time.deltaTime;
        Progress = Mathf.Clamp(Progress, 0, 100);
    }
    public void DecreaseProgress()
    {
        Progress -= _progressDecrement * Time.deltaTime;
        Progress = Mathf.Clamp(Progress, 0, 100);
    }
    void ClampTemp()
    {
        if (Temperature > 0) { Temperature = Mathf.Clamp(Temperature, 0, 50); }
        else { Temperature = Mathf.Clamp(Temperature, -50, 0); }
    }
}
