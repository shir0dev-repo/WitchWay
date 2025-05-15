using UnityEngine;

public enum State
{
    Whole,
    Chunky,
    Crumbly,
    Powder,
    Dust
}
public class StateOfIngredient : MonoBehaviour
{
    public State CurrState;
    public float durability = 100;

    private void Start()
    {
        CurrState = State.Whole;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "mortar-pestle")
        {
            TakeDamage(10);
            ChangeState();
        }
    }

    void TakeDamage(float dmg)
    {
        if (durability <= 0) { return; }
        durability -= dmg;
    }

    void ChangeState()
    {
        if (durability >= 80 && durability <= 100)
        {
            CurrState = State.Chunky;
        }
        if (durability >= 60 && durability <= 80)
        {
            CurrState = State.Crumbly;
        }
        if (durability >= 40 && durability <= 60)
        {
            CurrState = State.Powder;
        }
        if (durability <= 40)
        {
            CurrState = State.Dust;
        }

        Mathf.Clamp(durability, 0, 100);

        Debug.Log("Ingredient is currently: " + CurrState.ToString() + "\n"
            + "Ingredient's Durability: " + durability.ToString());

    }
}
