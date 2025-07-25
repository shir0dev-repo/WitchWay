using UnityEngine;

public class RandomArrowMovement : MonoBehaviour
{
    PotTemperature pot => PotTemperature.Instance;
    
    public float ArrowValue;
    float ArrowTargetValue; 
    public bool CanBeHeated, CanBeCooled;

    [SerializeField] float DefaultDuration;
    float TimeUntilDirectionSwitches;

    [SerializeField] float timeDuration;
    float timeElapsed;

    private void OnEnable()
    {
        PotTemperature.StartCooking += OnCookingStart;
    }
    void OnCookingStart()
    {
        ArrowValue = 0;
        SwitchArrowDirection();
        TimeUntilDirectionSwitches = DefaultDuration;
    }
    void Update()
    {
        if (!pot.currentlyCooking) return;
        
        TimeUntilDirectionSwitches -= Time.deltaTime;
        if (TimeUntilDirectionSwitches <= 0) 
        {
            TimeUntilDirectionSwitches = Random.Range(DefaultDuration--,DefaultDuration++);
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
        if (CanBeHeated == true && CanBeCooled == false) { ArrowTargetValue = Random.Range(5, 50); }
        else if (CanBeHeated == false && CanBeCooled == true) { ArrowTargetValue = Random.Range(-50, -5); }
        else if (CanBeHeated == true && CanBeCooled == true) { ArrowTargetValue = 0;}
        else { ArrowTargetValue = 0;}
        // last case just for now until i figure out how to make two arrows
    }
}
