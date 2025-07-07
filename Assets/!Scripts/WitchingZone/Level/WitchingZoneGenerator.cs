using EnumsEditor;
using DungeonMaster2D;
using UnityEngine;

public class WitchingZoneGenerator : MonoBehaviour
{
    [SerializeField] private EnumEditor<NodeType> NodeTypes;
    [SerializeField] private NodeType[] SpecialNodeTypes;
}