using DungeonMaster2D;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;
using DG.Tweening;

using Random = UnityEngine.Random;

public class WitchingZoneGenerator : MonoBehaviour
{
    [Serializable]
    public class RoomData
    {
        public GameObject Prefab;
        public Direction Entrances;

    }

    [Header("Visual")]
    [SerializeField] private bool _useCoroutine = false;
    [SerializeField, Range(0, 1.0f)] private float _roomSpawnDelay = 0.3f;
    [SerializeField] private float _roomFadeInDuration = 0.4f;

    [Header("Rooms")]
    [SerializeField] private float _roomExtentSize = 1.0f;
    [SerializeField] private float _roomScale = 5.0f;
    [SerializeField] private List<RoomData> _roomPrefabs;

    [Header("Generation")]
    [SerializeField] private Dungeon2D _dungeon;
    [SerializeField] private DungeonGeneratorData _generatorData;
    [SerializeField] private List<SpecialRoomType> _specialRoomTypes;

    private List<Room> _rooms = new();

    private void OnEnable()
    {
        GameEvents.WitchingZone.OnRoomExited += UnloadRoom;
        GameEvents.WitchingZone.OnRoomEntered += LoadRoom;
    }

    private void Start()
    {
        _dungeon = MapGenerator.Generate2D(_generatorData, _specialRoomTypes.Select(n => n.Type).ToArray());
        Random.InitState(_generatorData.GetSeed());

        StartCoroutine(GenerateDungeonCoroutine());
    }

    private IEnumerator GenerateDungeonCoroutine()
    {
        foreach (Node n in _dungeon.ValidNodes)
        {
            RoomData[] rooms = _roomPrefabs.Where(r => r.Entrances.HasAllFlags(n.Entrances)).ToArray();
            int rand_i = Random.Range(0, rooms.Length - 1);

            Vector3 pos = new Vector3(n.Position.x, 0, n.Position.y) * _roomExtentSize;
            GameObject pf = rooms[rand_i].Prefab;
            GameObject roomGO = Instantiate(pf, pos, pf.transform.rotation);
            roomGO.transform.SetParent(transform);
            roomGO.name = $"{n.NodeType} Room {n}";

            if (roomGO.TryGetComponent(out Room room))
            {
                room.Node = n;
                _rooms.Add(room);
            }

            if (_useCoroutine)
            {
                room.transform.localScale = Vector3.one * 0.001f;
                room.transform.DOScale(5.0f, _roomFadeInDuration);
                yield return new WaitForSeconds(_roomSpawnDelay);
            }
            else
            {
                yield return null;
            }
        }

        GameEvents.WitchingZone.OnDungeonGenerated?.Invoke(_dungeon);
    }

    private void LoadRoom(Room room)
    {
        
    }

    private void UnloadRoom(Room room)
    {
        StartCoroutine(UnloadRoomCoroutine(room));
    }

    private IEnumerator UnloadRoomCoroutine(Room room)
    {
        WZPlayerController player = WZPlayerController.Instance;
        if (player == null) yield break;

        player.SetCanMove(false);
        ScreenEffects se = ScreenEffects.Instance;
        if (se != null)
        {
            bool effectFinished = false;
            se.DoScreenEffect("Room Exit", 0.3f, 1.0f, () => effectFinished = true, false);
            yield return new WaitUntil(() => effectFinished);
        }
        Debug.Log("new room !!!");
    }
}