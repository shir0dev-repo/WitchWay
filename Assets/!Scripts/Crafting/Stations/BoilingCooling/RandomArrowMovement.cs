using UnityEngine;

public class RandomArrowMovement : MonoBehaviour
{
    PotTemperature pot => PotTemperature.Instance;

    public float TrueArrow
    {
        get
        {
            if (CanBeHeated && !CanBeCooled) return HeatArrow;
            else if (!CanBeHeated && CanBeCooled) return CoolArrow;
            else if (CanBeHeated && CanBeCooled)
            {
                float distH = Mathf.Abs(HeatArrow - pot.GetCurrentTemp());
                float distC = Mathf.Abs(CoolArrow - pot.GetCurrentTemp());
                return (distH <= distC) ? HeatArrow : CoolArrow;
            }

                return 0f;
        }
    }
    // if someone could do a better job with this then by all means pls do
    // i, myself, do not like this

    public float HeatArrow, CoolArrow;
    float HeatTarget, CoolTarget; 
    public bool CanBeHeated, CanBeCooled;

    [SerializeField] float DefaultDuration;
    float TimeUntilDirectionSwitches;

    [SerializeField] float timeDuration;
    float timeElapsed_H, timeElapsed_C;

    private void OnEnable()
    {
        PotTemperature.StartCooking += OnCookingStart;
    }
    void OnCookingStart()
    {
        HeatArrow = 0; CoolArrow = 0;
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
        }

        MoveArrow();
    }
    void MoveArrow()
    {
        if (HeatArrow != HeatTarget && CanBeHeated)
        {
            timeElapsed_H += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed_H / timeDuration);
            HeatArrow = Mathf.SmoothStep(HeatArrow, HeatTarget, t);
        }
        if (CoolArrow != CoolTarget && CanBeCooled)
        {
            timeElapsed_C += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed_C / timeDuration);
            CoolArrow = Mathf.SmoothStep(CoolArrow, CoolTarget, t);
        }
    }
    void SwitchArrowDirection()
    {
        Debug.Log("arrow should be switching now");

        timeElapsed_H = 0;
        timeElapsed_C = 0;

        GenerateRandomRangeNum();
    }
    void GenerateRandomRangeNum()
    {
        if (CanBeHeated && !CanBeCooled) { HeatTarget = Random.Range(5, 50); }
        else if (!CanBeHeated && CanBeCooled) { CoolTarget = Random.Range(-50, -5); }
        else if (CanBeHeated && CanBeCooled)
        {
            HeatTarget = Random.Range(5, 50);
            CoolTarget = Random.Range(-50, -5);
        }
        else { HeatTarget = 0; CoolTarget = 0; }
    }
    
}
