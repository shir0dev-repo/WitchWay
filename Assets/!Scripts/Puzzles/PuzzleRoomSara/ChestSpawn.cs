using UnityEngine;

public class ChestSpawn : PuzzleRewardBase
{
    [SerializeField] private GameObject chest;

    protected override void OnGiveReward()
    {
        if (chest != null)
        {
            chest.SetActive(true);
        }
    }
}
