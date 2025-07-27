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
        PotTemperature.FinishCooking += OnCookingEnd;
    }
    private void OnDisable()
    {
        PotTemperature.StartCooking -= OnCookingStart;
        PotTemperature.FinishCooking -= OnCookingEnd;
    }
    void OnCookingStart()
    {
        HeatArrow = 0; CoolArrow = 0;
        SwitchArrowDirection();
        TimeUntilDirectionSwitches = DefaultDuration;
    }
    void OnCookingEnd()
    {
        HeatArrow = 0; CoolArrow = 0;
        HeatTarget = 0; CoolTarget = 0;
        CanBeHeated = false; CanBeCooled = false;

        TimeUntilDirectionSwitches = 0;
        timeElapsed_H = 0; timeElapsed_C = 0;
    }
    void Update()
    {
        if (!pot.CurrentlyCooking) return;
        
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
        HeatTarget = CanBeHeated ? Random.Range(5, 45) : 0;
        CoolTarget = CanBeCooled ? Random.Range(-45, -5) : 0;
    }
}
