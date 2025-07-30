using UnityEngine;

/// <summary>
///     Base class for all WZ Puzzle Rewards.<br/>
/// </summary>
/// <remarks>
///     Inherit from this and implement <see cref="PuzzleRewardBase.OnGiveReward"/> to perform functionality when the player solves a puzzle.
/// </remarks>
public abstract class PuzzleRewardBase : MonoBehaviour, IPuzzleReward
{
    public bool HasRewardBeenGiven => _hasRewardBeenGiven;
    private bool _hasRewardBeenGiven = false;

    protected abstract void OnGiveReward();

    public void GiveReward()
    {
        if (_hasRewardBeenGiven) return;

        _hasRewardBeenGiven = true;
        OnGiveReward();
    }
}
