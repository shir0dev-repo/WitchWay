using UnityEngine;

public class Siphon : MonoBehaviour
{
    public static Siphon Instance {  get; private set; }
    [SerializeField] SliderBar slider;
    // siphon requires it's own slider

    [Header("Pressure Rates")]
    [SerializeField] private float _minFallSpeed = 5.0f;
    [SerializeField] private float _maxFallSpeed = 25.0f;

    [Header("Pressure Fill Speed")]
    [SerializeField] private float _addedPressure = 2.5f;
    [SerializeField] private float _maxPressureIncrease = 5.0f;
    [SerializeField] private float _pressureSmoothing = 0.75f;
    [SerializeField] float visiblePressure;

    Vector2 targetValue; // the value that's actually being calculated
    Vector2 pressureValue; // the value of the slider

    float currTime = 0.0f;

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

        this.enabled = false;
    }
    private void OnEnable()
    {
        BottleDetection.filledBottle += EndMinigame;
        BottleDetection.bottlePlaced -= StartMinigame;
    }
    private void OnDisable()
    {
        BottleDetection.filledBottle -= EndMinigame;
        BottleDetection.bottlePlaced += StartMinigame;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            IncreasePressure(_addedPressure);
        }

        DecreasePressure();
        slider.SetValue(EaseValues());

        visiblePressure = targetValue.x; // for debugging 
    }
    void IncreasePressure(float amount)
    {
        targetValue.x += amount;
        targetValue.x = ClampPressureAmount();

        currTime = 0f;
    } 
    float EaseValues()
    {
        float value;
        currTime += Time.deltaTime;

        float e = Mathf.Clamp01(currTime / 0.5f);
        value = Easing(e);
        // e = progress of the easing
        // value = the current value within the easing process

        pressureValue.x = Mathf.Lerp(pressureValue.x, targetValue.x, value);

        return pressureValue.x;
    }
    public void DecreasePressure()
    {
        float currentPercent = 1.0f - (targetValue.x * 0.01f);
        float decrease = Mathf.Lerp(_maxFallSpeed, _minFallSpeed, currentPercent);

        targetValue.x -= decrease * Time.deltaTime;
        targetValue.x = ClampPressureAmount();
    }
    float ClampPressureAmount()
    {
        return Mathf.Clamp(targetValue.x, 0, 100);
    }
    public float GetCurrentPressureAmount()
    {
        return targetValue.x;
    }
    public void ResetPressure() 
    {
        pressureValue.x = 0.0f;
        targetValue.x = 0.0f;
        slider.SetValue(targetValue.x);
    }
    void StartMinigame()
    {
        ResetPressure();
        slider.gameObject.SetActive(true);
    }
    void EndMinigame()
    {
        slider.gameObject.SetActive(false);
        
        this.enabled = false;
    }
    float Easing(float x) // taken from easings.net
    {
        return Mathf.Sin((x * Mathf.PI)/2);
    }
}
