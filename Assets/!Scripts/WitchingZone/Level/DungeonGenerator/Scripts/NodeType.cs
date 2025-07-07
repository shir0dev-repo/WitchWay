using System;
using System.Collections.Generic;
using System.Linq;

namespace DungeonMaster2D
{
    public enum NodeType
    {
        Start,
        Break,
        Travel,
        Stockpile,
        Discovery,
        Amazement,
        Scare,
        Puzzle,
        PuzzleReward,
        Fishing,
        Exit
    }

    public static class NodeTypeUtils
    {
        public static NodeType[] GetSpecialRoomTypes()
        {
            List<NodeType> nodes = new() { NodeType.Exit, NodeType.Discovery, NodeType.Amazement, NodeType.Scare, };
            return GetSpecialRoomTypes(nodes);
        }
        public static NodeType[] GetSpecialRoomTypes(IEnumerable<NodeType> ignoreTypes)
        {
            List<NodeType> roomTypes = new List<NodeType>(Enum.GetValues(typeof(NodeType)).Cast<NodeType>().ToArray());

            foreach (NodeType type in ignoreTypes)
            {
                roomTypes.Remove(type);
            }

            return roomTypes.ToArray();
        }
    }
}