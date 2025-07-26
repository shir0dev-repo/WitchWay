using System.Collections.Generic;
using UnityEngine;

public class StateOfIngredient_BurnCool : MonoBehaviour
{
    PotTemperature pot;

    public float targetTemp => pot.arrowMovement.TrueArrow;
    [SerializeField] float allowedDeviance = 10;

    bool isUsable = true;
    float numTimesBurnt = 0;
    [SerializeField] const float maxTimesBurnt = 3;

    private void OnEnable()
    {
        PotTemperature.FinishCooking += EndEnd;
        PotTemperature.TriggerBurning += OnBurning;
    }
    private void OnDisable()
    {
        PotTemperature.FinishCooking -= EndEnd;
        PotTemperature.TriggerBurning -= OnBurning;
    }
    void Start()
    {
        pot = PotTemperature.Instance;
    }
    void Update()
    {
        if (!pot.currentlyCooking) return;

        if (pot.Progress < 100)
        {
            CookIngredient(pot.Temperature);
        }
        else if (pot.Progress >= 100)
        {
            PotTemperature.FinishCooking?.Invoke();
        }
    }
    void EndEnd()
    {
        CalculateRating();
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
