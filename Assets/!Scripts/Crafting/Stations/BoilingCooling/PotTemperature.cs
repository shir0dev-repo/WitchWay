using UnityEngine;

public class PotTemperature : MonoBehaviour
{
    public static PotTemperature Instance {  get; private set; }

    public delegate void StartMinigame();
    public static StartMinigame StartCooking;

    public delegate void FinishMinigame();
    public static FinishMinigame FinishCooking;

    [SerializeField] Slider_WithPointer slider;

    public float TargetTemperature;
    public float Temperature = 0;
    public bool isChangingTemp = false;
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
    }
    private void OnDisable()
    {
        StartCooking = null;
        FinishCooking = null;
    }
    void Update()
    {
        if (slider.isActiveAndEnabled)
        {
            if (!isChangingTemp) { EqualOutTemp(); }
            // only runs when player is not hovering on button, prevents values from fighting

            slider.SetValue(Temperature);
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
        slider.gameObject.SetActive(true);
        slider.SetPointerLocation(TargetTemperature);
    }
    void EndEnd()
    {
        slider.gameObject?.SetActive(false);
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
    void ClampTemp()
    {
        if (Temperature > 0) { Temperature = Mathf.Clamp(Temperature, 0, 50); }
        else { Temperature = Mathf.Clamp(Temperature, -50, 0); }
    }
}
