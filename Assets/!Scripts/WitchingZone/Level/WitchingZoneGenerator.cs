using EnumsEditor;
using DungeonMaster2D;
using UnityEngine;

public class WitchingZoneGenerator : MonoBehaviour
{
    [SerializeField] private GameObject _roomPF;

    [SerializeField] private Dungeon2D _dungeon;
    [SerializeField] private DungeonGeneratorData _generatorData;

    private void Start()
    {
        _dungeon = MapGenerator.Generate2D(_generatorData, new NodeType[]
        {
            NodeType.Stockpile,
            NodeType.Amazement,
            NodeType.Scare
        });

        foreach (Node n in _dungeon.ValidNodes)
        {
            Vector3 pos = new Vector3(n.Position.x, 0, n.Position.y);
            GameObject room = Instantiate(_roomPF, pos, Quaternion.identity);
            room.transform.SetParent(transform);
            if (room.TryGetComponent(out MeshRenderer mr))
            {
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
    }
}