using UnityEngine;

public class FailState_BurnCool : MonoBehaviour
{
    /*
    thinking... 
    gonna try and do something with function swapping within events

    eg: when the button begins to burn, the functionality of onmouseenter/exit is turned to null 
    and replaced with new functionality to be written here or in the temp button script.
    i dunno, still thinking abt it.
    */


    float numberOfClicksNeeded;
    float numberOfClicksDone = 0;
    private void OnEnable()
    {
        PotTemperature.TriggerBurning += OnBurning;
    }
    private void OnDisable()
    {
        PotTemperature.TriggerBurning -= OnBurning;
    }
    private void OnMouseDown()
    {
        if (PotTemperature.Instance.amCurrentlyBurning)
        {
            numberOfClicksDone++;

            if (numberOfClicksDone >= numberOfClicksNeeded)
            {
                OnExtinguish();
            }
            Debug.Log("click");
            // this can be exchanged for something more fun later on
        }
    }
    void OnBurning()
    {
        numberOfClicksNeeded = Random.Range(1,10);
        // this can also be exchanged for something else
        // also fire effects can be added here
        Debug.Log("please click " + numberOfClicksNeeded + " times to extinguish the flame!");
    }
    void OnExtinguish()
    {
        numberOfClicksNeeded = 0;
        numberOfClicksDone = 0; 
        
        PotTemperature.Instance.amCurrentlyBurning = false;
        PotTemperature.Instance.ToggleSliders(true);
    }
}
