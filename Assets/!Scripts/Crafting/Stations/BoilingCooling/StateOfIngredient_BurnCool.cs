using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateOfIngredient_BurnCool : MonoBehaviour
{
    PotTemperature pot;

    public float targetTemp = 50;
    [SerializeField] float allowedDeviance = 5;
    [SerializeField] float cookingTime = 10; // by seconds
    [SerializeField] float averageTemp;

    bool canCook = false;

    List<float> ListOfTemperatures = new List<float>();
    private void OnEnable()
    {
        PotTemperature.StartCooking += StartStart;
        PotTemperature.FinishCooking += IngredientCookingTimeFinished;
    }
    private void OnDisable()
    {
        PotTemperature.StartCooking -= StartStart;
        PotTemperature.FinishCooking -= IngredientCookingTimeFinished;
    }
    void Start()
    {
        pot = PotTemperature.Instance;
    }
    void Update()
    {
        if (canCook)
        {
            if (cookingTime > 0)
            {
                cookingTime -= Time.deltaTime;
            }
            else if (cookingTime <= 0)
            {
                StopCoroutine(StoreCurrentTemp());
                averageTemp = GetAverageTemp();
                PotTemperature.FinishCooking?.Invoke();
            }
        }
    }
    void StartStart()
    {
        canCook = true;
        StartCoroutine(StoreCurrentTemp());
        // when the minigame starts, it starts recording the pot's temperature
        // via the coroutine
    }
    IEnumerator StoreCurrentTemp()
    {
        while (canCook == true)
        {
            ListOfTemperatures.Add(pot.GetCurrentTemp());
            yield return new WaitForSeconds(1f);
        }
    }
    // coroutine because i've always wanted to use these lmao
    float GetAverageTemp()
    {
        return ListOfTemperatures.Average();
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
        // this whole if/else statement can be replaced with more accurate endstates later

        canCook = false;
        ListOfTemperatures.Clear();
    }
    bool IsAverageTempInTargetRange()
    {
        return (targetTemp - allowedDeviance) < averageTemp && averageTemp < (targetTemp + allowedDeviance);
    }
}
