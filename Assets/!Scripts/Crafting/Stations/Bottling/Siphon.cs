using UnityEngine;

public class Siphon : MonoBehaviour
{
    public static Siphon instance {  get; private set; }

    public float pressureAmount = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            IncreasePressure();
        }

        pressureAmount -= Time.deltaTime;
    }

    public void IncreasePressure()
    {
        pressureAmount += 5;

        if ( pressureAmount > 100 )
        {
            pressureAmount = 100;
        }
    }
}
