using UnityEngine;

public class IngredientsInPot : MonoBehaviour
{
    int thingsInPot;
    int allIngredientsToAdd;

    void Start()
    {
        allIngredientsToAdd = GameObject.FindGameObjectsWithTag("Ingredient").Length;
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
