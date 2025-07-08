using EnumsEditor;
using DungeonMaster2D;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

public class WitchingZoneGenerator : MonoBehaviour
{
    [SerializeField] private GameObject _roomPF;
    [SerializeField] private Dungeon2D _dungeon;
    [SerializeField] private DungeonGeneratorData _generatorData;
    [SerializeField] private List<SpecialRoomType> _specialRoomTypes;

    public Action<Dungeon2D> OnDungeonGenerated;

    private void Start()
    {
        _dungeon = MapGenerator.Generate2D(_generatorData, _specialRoomTypes.Select(n => n.Type).ToArray());

        foreach (Node n in _dungeon.ValidNodes)
        {
            Vector3 pos = new Vector3(n.Position.x, 0, n.Position.y);
            GameObject room = Instantiate(_roomPF, pos, Quaternion.identity);
            room.transform.SetParent(transform);
            if (room.TryGetComponent(out MeshRenderer mr))
            {
                var special = _specialRoomTypes.Find(e => e.Type == n.NodeType);

                if (special != null)
                    mr.material.color = special.Color;
                else 
                    mr.material.color = n.NodeType switch
                    {
                        NodeType.Scare => Color.magenta,
                        NodeType.Stockpile => Color.green,
                        NodeType.Amazement => Color.yellow,
                        NodeType.Exit => Color.red,
                        _ => Color.white
                    };
            }
            else
            {
                Debug.Break();
            }
        }

        OnDungeonGenerated?.Invoke(_dungeon);
    }
}