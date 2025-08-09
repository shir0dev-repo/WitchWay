using System;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleRoomPainting : PuzzleBase
{
    [SerializeField] SnappableObject[] _paintingCollection;
    [SerializeField] List<GameObject> _paintings;

    int _NumOfCorrectPaintings = 0;
    bool _HasCorrectPaintings = false;

    private void Start()
    {
        foreach (var painting in _paintings)
        {
            if (!painting.TryGetComponent(out PaintingSnapping paint)) { return; }

            Array.Resize(ref _paintingCollection, _paintingCollection.Length + 1);
            _paintingCollection[_paintingCollection.Length - 1] = paint.GetSnappableObject();
        }
    }

    public void AddToCorrectPaintings()
    {
        _NumOfCorrectPaintings++;

        if (_NumOfCorrectPaintings == 3)
        {
            _HasCorrectPaintings = true;
            Debug.Log("has been solved!");
        }
    }
    public override bool IsSolved()
    {
        return _HasCorrectPaintings;
    }
}
