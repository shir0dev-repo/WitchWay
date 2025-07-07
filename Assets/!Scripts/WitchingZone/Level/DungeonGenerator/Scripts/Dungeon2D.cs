using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;

namespace DungeonMaster2D
{

    [Serializable]
    public class Dungeon2D : IEnumerable
    {
        public const int DIMENSIONS = 9;
        public const int MAX_SIZE = 81;

        [SerializeField] protected Node[] _nodes;
        protected int _nodeCount = 0;

        public int MaxNodes { get; private set; }
        public int MinNodes => MaxNodes - 3;

        public Node StartingNode
        {
            get { return _nodes[40]; }
        }

        public Node[] ValidNodes
        {
            get
            {
                Dungeon2DUtils.GetValidNodes(this, out Node[] validNodes);
                return validNodes;
            }
        }

        public Dungeon2D(DungeonGeneratorData data, Node startingNode)
        {
            _nodeCount = 0;
            _nodes = new Node[MAX_SIZE];
            MaxNodes = data.TargetRoomCount;

            AddNode(startingNode);
        }

        public Node this[int i]
        {
            get
            {
                try
                {
                    return _nodes[i];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
            private set
            {
                try
                {
                    _nodes[i] = value;
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public static implicit operator Node[](Dungeon2D dungeon) => dungeon._nodes;

        public Dungeon2DEnumerator GetEnumerator() => new(_nodes);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool InRange(Node node)
            => 0 <= node.x && node.x < DIMENSIONS &&
                0 <= node.y && node.y < DIMENSIONS;

        public bool Exists(Node node)
            => InRange(node) &&
                this[node.Index] != null &&
                this[node.Index].IsRoom;

        public bool AddNode(Node node)
        {
            if (_nodeCount + 1 <= MAX_SIZE && InRange(node))
            {
                this[node.Index] = node;
                _nodeCount++;
                return true;
            }

            else return false;
        }

        public void AssignEntrances()
        {
            foreach (Node node in ValidNodes)
            {
                node.SetNeighbourDirections(GetExistingNeighbours(node));
            }
        }

        public Node[] GetPseudoNeighbours(Node origin)
        {
            Node[] neighbours = new Node[4];

            for (int i = 0; i < 4; i++)
            {
                neighbours[i] = origin + Node.directions[i];
            }

            return neighbours;
        }

        public Node[] GetExistingNeighbours(Node origin)
        {
            Node[] pseudoNeighbours = GetPseudoNeighbours(origin);
            Node[] existingNeighbours = new Node[4];

            for (int i = 0; i < 4; i++)
            {
                if (Exists(pseudoNeighbours[i]))
                    existingNeighbours[i] = this[pseudoNeighbours[i].Index];
            }

            return existingNeighbours;
        }

        public Node[] GetDeadends(bool excludeStartingRoom = true)
        {
            List<Node> deadends = new();

            foreach (Node node in ValidNodes)
            {
                if (excludeStartingRoom && node == StartingNode)
                    continue;
                else if (GetExistingNeighbours(node).GetValidNodes() > 1)
                    continue;

                deadends.Add(node);
            }

            return deadends.ToArray();
        }

        public override string ToString()
        {
            return $"Node total: {ValidNodes.Length} " + '\n' +
                   $"Target node count: {MaxNodes}";
        }

        public void FinishGeneration()
        {
            if (_nodes == null) return;

            foreach (Node n in _nodes)
            {
                if (n != null)
                    n.SetName();
            }
        }
    }

    public class Dungeon2DEnumerator : IEnumerator
    {
        public Node[] nodes;
        int _position = -1;

        public Dungeon2DEnumerator(Node[] nodes) => this.nodes = nodes;

        public Node Current
        {
            get
            {
                try
                {
                    return nodes[_position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            _position++;
            return _position < nodes.Length;
        }

        public void Reset()
        {
            _position = -1;
        }
    }
}
