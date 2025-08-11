using DungeonMaster2D;
using System.Collections;
using UnityEngine;

public class DungeonManager : Singleton<DungeonManager>
{
    WZPlayerController _player;
    public Room CurrentRoom => _currentRoom;
    private Room _currentRoom;
    WitchingZoneGenerator _generator;
    private bool _canUpdate = true;

    private void OnEnable()
    {
        GameEvents.WitchingZone.OnDungeonGenerated += Initialize;
        GameEvents.WitchingZone.OnRoomEntered += SetCurrentRoom;
    }

    private void SetCurrentRoom(Room room)
    {
        room.HasBeenVisited = true;
        _currentRoom = room;
        _canUpdate = true;
    }

    private void OnDisable()
    {
        GameEvents.WitchingZone.OnDungeonGenerated -= Initialize;
    }

    private void Update()
    {
        if (!_canUpdate) return;

        if (_player != null)
        {
            Room current = _generator.GetRoom(_player.transform.position);
            if (current != null && current != _currentRoom)
            {
                _canUpdate = false;
                Vector3 currentPos = _currentRoom.transform.position;
                Vector3 targetPos = current.transform.position;
                Entrance e = Entrance.None;
                // x < WEST
                // x > EAST
                // z < SOUTH
                // z > NORTH
                if (targetPos.x < currentPos.x)
                    e = Entrance.West;
                else if (targetPos.x > currentPos.x)
                    e = Entrance.East;
                else if (targetPos.z < currentPos.z)
                    e = Entrance.South;
                else if (targetPos.z > currentPos.z)
                    e = Entrance.North;

                GameEvents.WitchingZone.OnRoomExited?.Invoke(_currentRoom, e);
            }
        }
    }

    private void Initialize(Dungeon2D d)
    {
        _generator = WitchingZoneGenerator.Instance;
        _currentRoom = _generator.GetRoom(d.StartingNode);

        StartCoroutine(PlayerSearchCoroutine());
    }

    private IEnumerator PlayerSearchCoroutine()
    {
        do
        {
            _player = FindFirstObjectByType<WZPlayerController>();
            yield return new WaitForSeconds(0.25f);
        } while (_player == null);
    }
}
