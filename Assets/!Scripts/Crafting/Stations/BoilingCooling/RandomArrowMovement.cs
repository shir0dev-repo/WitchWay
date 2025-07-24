using UnityEngine;

public class RandomArrowMovement : MonoBehaviour
{
    PotTemperature pot => PotTemperature.Instance;
    
    public float ArrowValue;
    float ArrowTargetValue; 
    [SerializeField] public bool CanBeHeated, CanBeCooled;

    [SerializeField] float TimeUntilDirectionSwitches = 4;
    [SerializeField] float timeDuration = 5;
    float timeElapsed;

    private void OnEnable()
    {
        ArrowValue = 0;
        SwitchArrowDirection();
    }

    void Update()
    {
        if (!pot.currentlyCooking) return;
        
        TimeUntilDirectionSwitches -= Time.deltaTime;
        if (TimeUntilDirectionSwitches <= 0) 
        {
            TimeUntilDirectionSwitches = Random.Range(3f,5f);
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
        GenerateRandomRangeNum();
    }
    void GenerateRandomRangeNum()
    {
        if (CanBeHeated && !CanBeCooled) { ArrowTargetValue = Random.Range(5, 50); }
        else if (!CanBeHeated && CanBeCooled) { ArrowTargetValue = Random.Range(-50, -5); }
        else if (CanBeHeated && CanBeCooled) { ArrowTargetValue = 0;}
        // last case just for now until i figure out how to make two arrows
    }
}
