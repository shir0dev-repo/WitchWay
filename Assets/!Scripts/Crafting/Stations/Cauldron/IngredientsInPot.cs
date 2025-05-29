using System.Collections.Generic;
using UnityEngine;

public class IngredientsInPot : MonoBehaviour
{
    public List<GameObject> IngredientsToAdd = new();
    int thingsInPot;
    int allIngredientsToAdd;

    void Start()
    {
        allIngredientsToAdd = IngredientsToAdd.Count;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ingredient"))
        {
            thingsInPot++;
            CheckPot();
        }
    }

    void CheckPot()
    {
        if (allIngredientsToAdd == thingsInPot)
        {
            Debug.Log("everything is in the pot!");
            SwitchToMixing.mixingMode?.Invoke();
        }
    }
}
