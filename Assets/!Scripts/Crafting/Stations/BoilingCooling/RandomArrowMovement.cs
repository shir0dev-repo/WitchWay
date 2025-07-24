using UnityEngine;

public class RandomArrowMovement : MonoBehaviour
{
    PotTemperature pot => PotTemperature.Instance;
    
    public float ArrowValue;
    float ArrowTargetValue; 
    public bool CanBeHeated, CanBeCooled;

    float TimeUntilDirectionSwitches = 4;
    float timeDuration = 5;
    float timeElapsed;

    private void OnEnable()
    {
        ArrowValue = 0;
        ArrowTargetValue = Random.Range(5, 50);
    }

    void Update()
    {
        if (!pot.currentlyCooking) return;
        
        TimeUntilDirectionSwitches -= Time.deltaTime;
        if (TimeUntilDirectionSwitches <= 0) 
        {
            TimeUntilDirectionSwitches = 4;
            SwitchArrowDirection();

            timeElapsed = 0;
        }

        MoveArrow();
    }
    void MoveArrow()
    {
        if (ArrowValue != ArrowTargetValue)
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / timeDuration);
            ArrowValue = Mathf.SmoothStep(ArrowValue, ArrowTargetValue, t);
        }
    }
    void SwitchArrowDirection()
    {
        Debug.Log("arrow should be switching now");
        ArrowTargetValue = Random.Range(5, 50);
    }
}
