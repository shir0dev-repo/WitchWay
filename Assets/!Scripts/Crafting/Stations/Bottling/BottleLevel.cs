using Unity.VisualScripting;
using UnityEngine;

public class BottleLevel : MonoBehaviour
{
    [SerializeField] SliderBar slider;
    [SerializeField] private float maxStreamRate = 15.0f;
    // slider for now until i figure out how to hook this thing up to the bottle

    public float amount = 0f;
    float rateOfStream = 0.0f;

    private void Start()
    {
        this.enabled = false;
    }
    void OnEnable()
    {
        BottleDetection.filledBottle += EndMinigame;
        BottleDetection.bottlePlaced -= StartMinigame;
    }
    void OnDisable()
    {
        BottleDetection.filledBottle -= EndMinigame;
        BottleDetection.bottlePlaced += StartMinigame;
    }

    void Update()
    {
        if (CheckIfFilled())
        {
            return;
            // checks if the bottle is filled via the function
        }
        RaiseStreamLevel();
        FillBottle();
        
        slider.SetValue(amount);
    }
    void RaiseStreamLevel()
    {
        float pressure = Siphon.Instance.GetCurrentPressureAmount();
        rateOfStream = Mathf.InverseLerp(0, 100, pressure) * maxStreamRate;
    }
    void FillBottle()
    {
        if (rateOfStream == 0)
        {
            return;
            // if the player hasn't pressed the button, the bottle won't fill up
        }
        amount += Time.smoothDeltaTime * rateOfStream;
       
        Mathf.Clamp(amount, 0, 100);
    }
    bool CheckIfFilled()
    {
        if (amount >= 100)
        {
            Debug.Log("The bottle is full!");
            BottleDetection.filledBottle?.Invoke();
            return true;
            // later, add stuff to disable the pump if the bottle is full
        }
        return false;
    }
    void EndMinigame()
    {
        slider.gameObject.SetActive(false);
        this.enabled = false;
    }
    void StartMinigame()
    {
        slider.gameObject.SetActive(true);

        amount = 0;
        rateOfStream = 0.0f;
    }
}
