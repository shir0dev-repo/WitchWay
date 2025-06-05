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

    public float pressureAmount = 0.0f;
    float currTime = 0.0f;
    bool canPressButton = true;

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
        if (Input.GetKeyDown(KeyCode.Space) && canPressButton)
        {
            IncreasePressure(_addedPressure);
        }
        if (canPressButton)
        {
            DecreasePressure();
        }

        slider.SetValue(pressureAmount);
    }
    void IncreasePressure(float amount)
    {
        pressureAmount += amount;
        pressureAmount = ClampPressureAmount();
    }
    public void DecreasePressure()
    {
        float currentPercent = 1.0f - (pressureAmount * 0.01f);
        float decrease = Mathf.Lerp(_maxFallSpeed, _minFallSpeed, currentPercent);

        pressureAmount -= decrease * Time.deltaTime;
        pressureAmount = ClampPressureAmount();
    }
    float ClampPressureAmount()
    {
        return Mathf.Clamp(pressureAmount, 0, 100);
    }
    public float GetCurrentPressureAmount()
    {
        return pressureAmount;
    }
    public void ToggleButtonAbility()
    {
        canPressButton = !canPressButton;
        ResetPressure();
    }
    public void ResetPressure()
    {
        pressureAmount = 0.0f;
        slider.SetValue(pressureAmount);
    }
    void StartMinigame()
    {
        canPressButton = true;
        ResetPressure();
        slider.gameObject.SetActive(true);
    }
    void EndMinigame()
    {
        canPressButton = false;
        slider.gameObject.SetActive(false);
        
        this.enabled = false;
    }
    float EaseIncrease(float x) // taken from easings.net
    {
        return Mathf.Sin((x * Mathf.PI)/2);
    }
}
