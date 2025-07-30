using System;
using UnityEngine;

public class PuzzleManager : Singleton<PuzzleManager>
{
    private PuzzleBase _currentPuzzle = null;

    private void OnEnable()
    {
        GameEvents.WitchingZone.OnRoomEntered += CheckForPuzzle;
    }

    private void OnDisable()
    {
        GameEvents.WitchingZone.OnRoomEntered -= CheckForPuzzle;
    }

    private void Update()
    {
        if (_currentPuzzle == null)
            return;
        else if (_currentPuzzle.HasBeenSolved)
            return;

        if (_currentPuzzle.IsSolved())
        {
            _currentPuzzle.SolvePuzzle();
            _currentPuzzle = null;
        }
    }

    private void CheckForPuzzle(Room room)
    {
        if (!room.TryGetComponent(out _currentPuzzle))
            return;

        else if (_currentPuzzle.HasBeenSolved)
            return;
    }
}
