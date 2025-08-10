using UnityEngine;

public class PaintingRoomReward : PuzzleRewardBase
{
    [SerializeField] GameObject _treasureChest;
    
    protected override void OnGiveReward()
    {
        Debug.Log("chest should been spawned in now.");

        _treasureChest.SetActive(true);
    }
}
