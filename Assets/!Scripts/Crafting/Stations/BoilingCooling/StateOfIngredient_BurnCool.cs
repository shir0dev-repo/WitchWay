using System.Collections.Generic;
using UnityEngine;

public class StateOfIngredient_BurnCool : MonoBehaviour
{
    PotTemperature pot;

    public float targetTemp;
    [SerializeField] float allowedDeviance = 5;
    bool canCook = false;

    bool isUsable = true;
    float numTimesBurnt = 0;
    [SerializeField] const float maxTimesBurnt = 3;

    //private WorldIngredient _currentIngredient = null;

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
        PotTemperature.TriggerBurning -= OnBurning;
    }
    void Start()
    {
        pot = PotTemperature.Instance;
        targetTemp = Random.Range(-45, 45);
    }
    void Update()
    {
        if (!canCook) return;

        if (pot.Progress < 100)
        {
            CookIngredient(pot.Temperature);
        }
        else if (pot.Progress >= 100)
        {
            PotTemperature.FinishCooking?.Invoke();
        }
    }
    void StartStart()
    {
        canCook = true;
    }

    void EndEnd()
    {
        CalculateRating();

        canCook = false;
    }
    void CookIngredient(float currTemp)
    {
        if (IsTempInTargetRange(currTemp)) { pot.IncreaseProgress(); }
        else { pot.DecreaseProgress(); }
    }
    void OnBurning()
    {
        numTimesBurnt++;

        if (numTimesBurnt >= maxTimesBurnt)
        {
            isUsable = false;
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
