using System;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleRoomPainting : PuzzleBase
{
    [SerializeField] SnappableObject[] _paintingCollection;
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
        return false;
    }
}
