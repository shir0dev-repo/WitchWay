using UnityEngine;
using DungeonMaster2D;

public interface IRoomType
{
    NodeType Type { get; }
    bool PlacementRequirements(Dungeon2D dungeon, Node[] adjacentRooms);
}
