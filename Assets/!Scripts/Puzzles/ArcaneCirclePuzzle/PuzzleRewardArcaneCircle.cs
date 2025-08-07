using UnityEngine;
using UnityEngine.UIElements;

public class PuzzleRewardArcaneCircle : PuzzleRewardBase
{
    [Header("Dragonscale Reward")]
    [SerializeField] private IngredientSO dragonscaleIngredient; // Reference to the dragonscale ScriptableObject
    [SerializeField] private int dragonscaleAmount = 1;
    [SerializeField] private GameObject chestPrefab;
    [SerializeField] private GameObject chestSpawnPoint;

    protected override void OnGiveReward()
    {
        Debug.Log("Giving dragonscale to player");
        
        if (chestPrefab != null)
        {
            Instantiate(chestPrefab, chestSpawnPoint.transform.position, Quaternion.identity);
        }
        else
        {
            Debug.Log("Chest prefab is not assigned in PuzzleRewardArcaneCircle.");
        }

        if (dragonscaleIngredient != null)
        {
            Inventory playerInventory = WZPlayerManager.Instance.GetComponent<Inventory>();
            if (playerInventory != null)
            {
                for (int i = 0; i < dragonscaleAmount; i++)
                {
                    playerInventory.AddNewItem(dragonscaleIngredient);
                }
                Debug.Log($"Added {dragonscaleAmount} dragonscale(s) to player inventory");
            }
            else
            {
                Debug.Log("Player inventory not found!");
            }
        }
        else
        {
            Debug.Log("Dragonscale ingredient ScriptableObject is not assigned!");
        }
    }
}