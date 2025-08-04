using System;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleRoomPainting : PuzzleBase
{
    SnappableObject[] _paintingCollection = new SnappableObject[0];
    [SerializeField] List<GameObject> _paintings;

    private void Start()
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
        foreach(SnappableObject painting in _paintingCollection)
        {
            if (painting.Object.gameObject)
            {
                return true; // placeholder for actual behaviour
            }
        }

        return false;
    }
}
