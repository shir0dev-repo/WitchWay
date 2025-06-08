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
    
    void Start()
    {
        pot = PotTemperature.Instance;
    }
    void Update()
    {
        
    }
    void IngredientCookingTimeFinished()
    {
        if (CompareTargetAndPlayerAverageTemp())
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
    bool CompareTargetAndPlayerAverageTemp()
    {
        return (targetTemp + allowedDeviance) > averageTemp || (targetTemp - allowedDeviance) < averageTemp;
        // gonna change this later ofc
    }
}
