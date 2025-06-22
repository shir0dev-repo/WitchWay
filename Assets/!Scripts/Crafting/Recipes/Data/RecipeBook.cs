using UnityEngine;

public class RecipeBook : MonoBehaviour
{
    public static RecipeBook Instance {  get; private set; }
    public RecipeList list {  get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        list = GetComponentInChildren<RecipeList>();
    }
}
