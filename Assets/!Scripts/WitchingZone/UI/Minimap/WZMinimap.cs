using DungeonMaster2D;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WZMinimap : MonoBehaviour
{
    class RoomIcon
    {
        public RectTransform transform;
        public Vector2 position;
        public Room room;
        public Image sprite;

        public RoomIcon(RectTransform rectTransform, Vector2 position, float width, float height, Room room)
        {
            transform = rectTransform;
            transform.anchoredPosition = position;
            transform.sizeDelta = new Vector2(width, height);
            sprite = transform.GetComponent<Image>();
            this.position = position;
            this.room = room;
        }
    }

    [Header("Visuals")]
    [SerializeField] private Vector2Int _dimensions = Vector2Int.one * 256;
    [SerializeField] private int _roomSpacing;
    [SerializeField] private GameObject _roomIconPF;
    [SerializeField] private GameObject _playerIconPF;
    private Dictionary<Node, RoomIcon> _roomIcons = new();

    private Dungeon2D _dungeon;
    private RectTransform _playerIcon;
    private void OnEnable()
    {
        GameEvents.WitchingZone.OnDungeonGenerated += InitializeMinimap;
        GameEvents.WitchingZone.OnRoomEntered += UpdateMinimap;
    }

    

    private void OnDisable()
    {
        GameEvents.WitchingZone.OnDungeonGenerated -= InitializeMinimap;
        GameEvents.WitchingZone.OnRoomEntered -= UpdateMinimap;
    }

    private void Start()
    {
        GetComponent<RectTransform>().sizeDelta = _dimensions;
    }

    private void InitializeMinimap(Dungeon2D dungeon)
    {
        _dungeon = dungeon;

        int width = _dimensions.x / dungeon.ValidNodes.OrderByDescending(n => n.Position.x).First().x;
        int height = _dimensions.y / dungeon.ValidNodes.OrderByDescending(n => n.Position.y).First().y;
        
        int count = dungeon.ValidNodes.Length;
        RoomIcon startingRoomIcon = null;
        Node[] startNeighbours = dungeon.GetExistingNeighbours(dungeon.StartingNode);
        for (int i = 0; i < count; i++)
        {
            Node current = dungeon.ValidNodes[i];

            RectTransform rt = Instantiate(_roomIconPF, transform).GetComponent<RectTransform>();
            Room currentRoom = WitchingZoneGenerator.Instance.GetRoom(current);
            Vector2 position = new Vector2((width + _roomSpacing) * current.x - _dimensions.x * 0.5f, (height + _roomSpacing) * current.y - _dimensions.y * 0.5f);
            
            RoomIcon ico = new RoomIcon(rt, position, width, height, currentRoom);
            _roomIcons.Add(current, ico);

            if (current == dungeon.StartingNode)
            {
                startingRoomIcon = ico;
                continue;
            }
            else if (startNeighbours.Contains(current))
                ico.sprite.color = new Color(1, 1, 1, 0.25f);
            else
                ico.sprite.color = Color.clear;
        }

        _playerIcon = Instantiate(_playerIconPF, transform).GetComponent<RectTransform>();
        _playerIcon.sizeDelta = new Vector2(width * 0.75f, height * 0.75f);
        _playerIcon.anchoredPosition = startingRoomIcon.position;
    }

    private void UpdateMinimap(Room room)
    {
        if (_dungeon == null || room == null) return;
        if (!_roomIcons.TryGetValue(room.Node, out RoomIcon ico)) return;

        ico.sprite.color = Color.white;
        _playerIcon.anchoredPosition = ico.position;

        var neighbours = _dungeon.GetExistingNeighbours(room.Node).Where(n => n != null);

        foreach (Node n in neighbours)
        {
            if (_roomIcons.TryGetValue(n, out RoomIcon neighbourIcon))
            {
                
                if (neighbourIcon.room == null) throw new NullReferenceException();

                if (neighbourIcon.room.HasBeenVisited) continue;

                neighbourIcon.sprite.color = new Color(1, 1, 1, 0.25f);
            }
        }
    }
}
