using UnityEngine;
using DungeonMaster2D;
using System.Collections.Generic;

[System.Flags]
public enum Entrance : byte
{
    None = 0,
    North = 1,
    East = 2,
    South = 4,
    West = 8
};

public class Room : MonoBehaviour
{
    public Node Node { get; set; } = null;
    public Door[] Doors { get; private set; }
    public bool HasBeenVisited { get; set; } = false;

    [SerializeField]
    private List<Transform> enemySpawnPoints = new();
    public List<Transform> EnemySpawnPoints => enemySpawnPoints;
    public List<WZChaseAI> ChaseEnemies { get; private set; } = new();
    public List<WZMimicAI> MimicEnemies { get; private set; } = new();

    private void OnEnable()
    {
        GameEvents.WitchingZone.OnDungeonGenerated += Setup;
    }

    private void OnDisable()
    {
        GameEvents.WitchingZone.OnDungeonGenerated -= Setup;
    }

    private void Setup(Dungeon2D d)
    {
        if (Node == null)
        {
            Debug.LogWarning("A room does not have a Node attached to it!");
            return;
        }
        
        InitDoors();
        if (Node.NodeType == NodeType.Start) HasBeenVisited = true;

        if (WitchingZoneGenerator.Instance == null || !WitchingZoneGenerator.Instance.ShouldDisableOnStart) return;

        if (Node.NodeType != NodeType.Start)
        {
            gameObject.SetActive(false);
        }
    }

    private void InitDoors()
    {
        Doors = GetComponentsInChildren<Door>(true);
        foreach (Door door in Doors) door.AttachedRoom = this;
    }
}
