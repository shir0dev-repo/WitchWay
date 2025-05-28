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
    private void OnCollisionEnter(Collision other) // changed from trigger to prevent multiple calls while mashing
    {
        if (other.gameObject.TryGetComponent(out Pestle pestle))
        {
            TakeDamage(5);
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
        if (durability > 70)
        {
            CurrState = State.Chunky;
        }
        if (durability > 40)
        {
            CurrState = State.Crumbly;
        }
        if (durability > 10)
        {
            CurrState = State.Powder;
        }
        if (durability <= 10)
        {
            CurrState = State.Dust;
        }

        Mathf.Clamp(durability, 0, 100);

        Debug.Log("Ingredient is currently: " + CurrState.ToString() + "\n"
            + "Ingredient's Durability: " + durability.ToString());

    }
}
