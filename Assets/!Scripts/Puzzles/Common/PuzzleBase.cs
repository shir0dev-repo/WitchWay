using UnityEngine;

/// <summary>
///     Base class of all WZ Puzzles.
/// </summary>
/// <remarks>
///     Inherit from this and implement <c><see cref="IsSolved"/></c> to check if the player has solved the puzzle.
/// </remarks>
public abstract class PuzzleBase : MonoBehaviour
{
    [SerializeField] protected Room _room;
    [SerializeField] protected PuzzleRewardBase _reward;

    public bool HasBeenSolved => _hasBeenSolved;
    private bool _hasBeenSolved = false;

    protected virtual void Awake()
    {
        _room = GetComponent<Room>();
        _reward = GetComponent<PuzzleRewardBase>();
    }

    /// <summary>
    ///     Called from <see cref="PuzzleManager"/> when the player is inside this puzzles room, until it is solved.
    /// </summary>
    /// <remarks>This method is called every frame the player is inside the Witching Zone; optimization is key here.</remarks>
    /// <returns>
    ///     <c>true</c>: Puzzle was solved by player.<br/>
    ///     <c>false</c>: Puzzle has not been solved by player.
    /// </returns>
    public abstract bool IsSolved();

    /// <summary>
    ///     Called once, when the player first solves the puzzle.
    /// </summary>
    /// <remarks>
    ///     This should <b>NOT</b> be used to give rewards, as that method is called from within <see cref="SolvePuzzle"/>.<br/>
    ///     Use this method to do cleanup work on the rooms such as resetting visuals or disabling GameObjects, or instantiating effects.
    /// </remarks>
    protected virtual void OnSolvePuzzle() { }

    /// <summary>
    ///     Called from <see cref="PuzzleManager"/> once the player solves the puzzle.
    /// </summary>
    /// <remarks>
    ///     To add additional functionality to this event, implement <c><see cref="OnSolvePuzzle"/></c>.
    /// </remarks>
    public void SolvePuzzle()
    {
        if (_hasBeenSolved) return;

        _hasBeenSolved = true;
        OnSolvePuzzle();
        if (_reward == null)
        {
            Debug.LogError($"{_room.gameObject.name} does not have a puzzle reward!");
            return;
        }

        _reward.GiveReward();
        GameEvents.WitchingZone.OnPuzzleSolved?.Invoke(_room, _reward);
    }
}
