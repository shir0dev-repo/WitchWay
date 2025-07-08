using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using UnityEngine;

namespace DungeonMaster2D
{
  internal static class Dungeon2DUtils
  {
    public static Node[] GetValidNodes(this Node[] nodeCollection)
    {
      List<Node> validList = new();

      foreach (Node node in nodeCollection)
      {
        if (node != null && node.IsRoom)
          validList.Add(node);
      }
      return validList.ToArray();
    }
  }
}