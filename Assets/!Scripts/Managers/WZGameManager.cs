using DungeonMaster2D;
using System;
using UnityEngine;

public class WZGameManager : Singleton<WZGameManager>
{
    //[SerializeField] private GameObject _playerPrefab;
    [SerializeField] private Transform _playerPrefab;

    private void OnEnable()
    {
        GameEvents.WitchingZone.OnDungeonGenerated += SpawnPlayer;
    }

    private void OnDisable()
    {
        GameEvents.WitchingZone.OnDungeonGenerated -= SpawnPlayer;
    }

    private void SpawnPlayer(Dungeon2D dungeon)
    {
        Vector2Int spawnRoom = dungeon.StartingNode.Position;
        Vector3 spawnPosition = new Vector3(spawnRoom.x, 0f, spawnRoom.y);
        spawnPosition = Vector3.Scale(spawnPosition, WitchingZoneGenerator.Instance.RoomScale);
        spawnPosition += Vector3.up * 2.0f;
        //Instantiate(_playerPrefab, spawnPosition, Quaternion.identity);
        _playerPrefab.position = spawnPosition;
        _playerPrefab.rotation = Quaternion.identity;
        _playerPrefab.gameObject.SetActive(true);
        GameEvents.WitchingZone.OnPlayerSpawned?.Invoke();
    }
}
