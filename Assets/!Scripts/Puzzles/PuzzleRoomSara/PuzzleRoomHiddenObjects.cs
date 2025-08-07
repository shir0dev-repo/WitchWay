using UnityEngine;

public class PuzzleRoomHiddenObjects : PuzzleBase
{
    [SerializeField] private SnappableObject[] _objectCollection;

    public override bool IsSolved()
    {
        if (HasBeenSolved) { return true; }

        foreach (SnappableObject obj in _objectCollection)
        {
            if (!obj.IsWithinSnapRange()) { return false; }
        }

        SolvePuzzle();
        return true;
    }

    protected override void OnSolvePuzzle()
    {
        Debug.Log("Yuh");
    }
}
