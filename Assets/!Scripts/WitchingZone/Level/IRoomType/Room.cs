using UnityEngine;
using DungeonMaster2D;

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
    [Header("Room Data")]
    public Entrance Entrances => _entrances;
    [SerializeField] Entrance _entrances;
    
    public NodeType Type => _roomType;
    [SerializeField] NodeType _roomType;

    [Header("Scene References")]
    public Door[] Doors => _doors;
    [SerializeField] private Door[] _doors;

    [ContextMenu("Set Entrances")]
    public void SetEntrances()
    {
        if (_doors == null)
        {
            _doors = GetComponentsInChildren<Door>(true);
        }

        int entrances = 0;

        foreach (Door door in _doors)
        {
            Vector3 toDoor = (door.transform.position - transform.position).normalized;
            int angle = (int) Vector3.SignedAngle(transform.forward, toDoor, Vector3.up);
            entrances |= Mathf.NextPowerOfTwo(angle / 90);
        }

        _entrances = (Entrance) entrances;
    }
}
