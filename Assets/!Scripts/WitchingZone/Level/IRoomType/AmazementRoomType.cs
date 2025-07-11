using DungeonMaster2D;
using System.Linq;
using UnityEngine;

public class AmazementRoomType : IRoomType
{
    public NodeType Type => NodeType.Amazement;

    public bool PlacementRequirements(Dungeon2D dungeon, Node[] adjacentRooms)
    {
        // Amazement room already exists
        if (dungeon.ValidNodes.FirstOrDefault(n => n.NodeType == Type) != null)
            return false;
        else return true;
    }
}
