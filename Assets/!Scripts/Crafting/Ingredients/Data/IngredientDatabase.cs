using UnityEngine;

[CreateAssetMenu(fileName = "IngredientDB", menuName = "Alchemy/New IngredientDB")]
public class IngredientDatabase : SODatabase.ScriptableObjectDatabase<IngredientSO>
{
    [ContextMenu("Find All")]
    public override void Find()
    {
        FindAll();
    }
}
