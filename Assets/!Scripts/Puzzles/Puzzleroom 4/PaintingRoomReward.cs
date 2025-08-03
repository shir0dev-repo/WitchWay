using UnityEngine;

public class PaintingRoomReward : PuzzleRewardBase
{
    protected override void OnGiveReward()
    {
        Debug.Log("Player should be getting a dragonscale and amethyst.");
    }
}
