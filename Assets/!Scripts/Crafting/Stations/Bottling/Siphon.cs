using UnityEngine;

public class Siphon : MonoBehaviour
{
    public static Siphon instance {  get; private set; }
    [SerializeField] SliderBar slider;
    // siphon requires it's own slider

    public float pressureAmount = 0;
    bool canPressButton = true;

    public delegate void FinishMinigame();
    public static FinishMinigame filledBottle;
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
    private void OnEnable()
    {
        filledBottle += EndMinigame;
    }
    private void OnDisable()
    {
        filledBottle -= EndMinigame;
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
    }
    public void ResetPressure()
    {
        pressureAmount = 0;
        slider.SetValue(pressureAmount);
    }
    void EndMinigame()
    {
        canPressButton = false;
        ResetPressure();
        slider.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
