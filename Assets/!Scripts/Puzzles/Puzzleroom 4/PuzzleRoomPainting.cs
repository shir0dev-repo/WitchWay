using System;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleRoomPainting : PuzzleBase
{
    [SerializeField] SnappableObject[] _paintingCollection;
   
    public override bool IsSolved()
    {
        return false;
    }
}
