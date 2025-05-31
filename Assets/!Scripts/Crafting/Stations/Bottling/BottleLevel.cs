using Unity.VisualScripting;
using UnityEngine;

public class BottleLevel : MonoBehaviour
{
    [SerializeField] SliderBar slider;
    // slider for now until i figure out how to hook this thing up to the bottle

    public float amount = 0f;
    float rateOfStream = 0f;

    void Update()
    {
        if (CheckIfFilled())
        {
            return;
        }
        else
        {
            RaiseBottleLevel();
            amount += Time.deltaTime * rateOfStream;
            Mathf.Clamp(amount, 0f, 100);
        }
        
        slider.SetValue(amount);
    }
    void RaiseBottleLevel()
    {
        float i = Siphon.instance.GetCurrentPressureAmount();

        if (i < 25) { rateOfStream = 1; }
        else if (i > 50) { rateOfStream = 5; }
        else if (i > 75) { rateOfStream = 15; }
    }
    bool CheckIfFilled()
    {
        if (amount >= 100)
        {
            Debug.Log("The bottle is full!");
            return true;
            // later, add stuff to disable the pump if the bottle is full
        }
        else
        {
            return false;
        }
    }
}
