using DungeonMaster2D;
using System;
using System.Collections;
using UnityEngine;

public class DungeonManager : Singleton<DungeonManager>
{
    WZPlayerController _player;
    private Room _currentRoom;
    WitchingZoneGenerator _generator;

    private void OnEnable()
    {
        GameEvents.WitchingZone.OnDungeonGenerated += Initialize;
    }

    private void OnDisable()
    {
        GameEvents.WitchingZone.OnDungeonGenerated -= Initialize;
    }

    private void Update()
    {
        if (_player != null)
        {
            Room current = _generator.GetRoom(_player.transform.position);
            if (current != _currentRoom)
            {
                GameEvents.WitchingZone.OnRoomExited?.Invoke(_currentRoom);
                current.HasBeenVisited = true;
                GameEvents.WitchingZone.OnRoomEntered?.Invoke(current);
                _currentRoom = current;
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
