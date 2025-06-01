using Unity.VisualScripting;
using UnityEngine;

public class BottleLevel : MonoBehaviour
{
    [SerializeField] SliderBar slider;
    // slider for now until i figure out how to hook this thing up to the bottle

    public float amount = 0f;
    float rateOfStream = 0f;

    void OnEnable()
    {
        Siphon.filledBottle += EndMinigame;
    }
    void OnDisable()
    {
        Siphon.filledBottle -= EndMinigame;
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
        float i = Siphon.instance.GetCurrentPressureAmount();

        if (i == 0) { rateOfStream = 0; }
        else if (i <= 25) { rateOfStream = 1; }
        else if (i <= 50) { rateOfStream = 5; }
        else if (i <= 80) { rateOfStream = 10; }
        else { rateOfStream = 15; }
        // if there's a better way to do this please replace this
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
            Siphon.filledBottle?.Invoke();
            return true;
            // later, add stuff to disable the pump if the bottle is full
        }
        return false;
    }
    void EndMinigame()
    {
        slider.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
