using System.Collections.Generic;
using UnityEngine;

public class StateOfIngredient_BurnCool : MonoBehaviour
{
    PotTemperature pot;

    public float targetTemp = 0;
    [SerializeField] float allowedDeviance = 5;
    [SerializeField] float cookingTime = 60; // 60 seconds, could be more
    [SerializeField] float averageTemp;

    float minTemp;
    float maxTemp;

    List<float> ListOfTemperatures = new List<float>();
    
    void Start()
    {
        pot = PotTemperature.Instance;
    }
    void Update()
    {
        if (cookingTime > 0)
        {
            cookingTime -= Time.deltaTime;
            StoreCurrentTemp();
        }
    }
    void StoreCurrentTemp()
    {
        ListOfTemperatures.Add(pot.GetCurrentTemp());
    }
    void GetMinAndMax()
    {
         
    }
    void GetAverageTemp()
    {
        ListOfTemperatures.Clear();
    }
    void IngredientCookingTimeFinished()
    {
        if (IsAverageTempInTargetRange())
        {
            if (Mathf.Sign(averageTemp) == 1)
            {
                Debug.Log("You cooked the ingredient correctly!");
            }
            else { Debug.Log("You cooled the ingredient correctly!"); }
        }
        else
        {
            if (Mathf.Sign(averageTemp) == 1)
            {
                Debug.Log("YOU BURNT IT TO A CRISP");
            }
            else { Debug.Log("ITS A FROZEN BLOCK"); }
        }
    }
    bool IsAverageTempInTargetRange()
    {
        return (targetTemp + allowedDeviance) > averageTemp || (targetTemp - allowedDeviance) < averageTemp;
        // gonna change this later ofc
    }
}
