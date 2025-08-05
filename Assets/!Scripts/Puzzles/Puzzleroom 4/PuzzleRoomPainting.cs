using System;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleRoomPainting : PuzzleBase
{
    SnappableObject[] _paintingCollection = new SnappableObject[0];
    [SerializeField] List<GameObject> _paintings;

    private void OnEnable()
    {
        foreach (var painting in _paintings)
        {
            if (!painting.TryGetComponent(out PaintingSnapping paint)) { return; }
            
            Array.Resize(ref _paintingCollection, _paintingCollection.Length + 1);
            _paintingCollection[_paintingCollection.Length - 1] = paint.GetSnappableObject();
        }
    }
    public override bool IsSolved()
    {
        List<bool> bools = new List<bool>();

        // checks all the paintings in the inspector, adds their state into a list
        // if all of the paintings are in the correct position, the function returns true
        foreach(var painting in _paintings)
        {
            if (!painting.TryGetComponent(out PaintingSnapping paint)) { return false; }

            if (paint._isInCorrectPosition) { bools.Add(true); }
            else { bools.Add(false); }
        }

        if (bools.TrueForAll(x => x))
        {
            Debug.Log("the puzzle has been solved!");
            return true;
        }
        return false;
    }
}
