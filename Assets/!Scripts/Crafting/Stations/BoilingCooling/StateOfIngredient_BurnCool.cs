using System.Collections.Generic;
using UnityEngine;

public class StateOfIngredient_BurnCool : MonoBehaviour
{
    PotTemperature pot;

    public float targetTemp;
    [SerializeField] float allowedDeviance = 5;
    bool canCook = false;

    List<float> ListOfTemperatures = new List<float>();

    bool isUsable = true;
    float numTimesBurnt = 0;
    private void OnEnable()
    {
        PotTemperature.StartCooking += StartStart;
        PotTemperature.FinishCooking += EndEnd;
        PotTemperature.TriggerBurning += OnBurning;
    }
    private void OnDisable()
    {
        PotTemperature.StartCooking -= StartStart;
        PotTemperature.FinishCooking -= EndEnd;
        PotTemperature.TriggerBurning += OnBurning;
    }
    void Start()
    {
        pot = PotTemperature.Instance;
    }
    void Update()
    {
        if (canCook)
        {
            if (pot.Progress < 100)
            {
                if (IsTempInTargetRange(pot.Temperature)) { pot.IncreaseProgress(); }
                else { pot.DecreaseProgress(); }
            }
            else if (pot.Progress >= 100)
            {
                PotTemperature.FinishCooking?.Invoke();
            }
        }
    }
    void StartStart()
    {
        canCook = true;

        targetTemp = Random.Range(-50, 50);
        pot.TargetTemperature = targetTemp;
    }
    void EndEnd()
    {
        CalculateRating();

        canCook = false;
        ListOfTemperatures.Clear();
    }
    void OnBurning()
    {
        numTimesBurnt++;

        if (numTimesBurnt >= 3)
        {
            isUsable = false;
            PotTemperature.FinishCooking?.Invoke();
        }
    }
    void CalculateRating()
    {
        if (isUsable)
        {
            Debug.Log("yay");
        }
        else { Debug.Log("the ingredient is unusable..."); }
    }
    bool IsTempInTargetRange(float temp)
    {
        return (targetTemp - allowedDeviance) < temp && temp < (targetTemp + allowedDeviance);
    }
}
