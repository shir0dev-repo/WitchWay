using System.Runtime.InteropServices;
using UnityEngine;

public class WZScareRoomManager : MonoBehaviour
{
    [SerializeField] WZDialogueMonsterAI dialogueMonsterAI;

    private WZScareRoom currentScareRoom;

    void OnEnable()
    {
        GameEvents.WitchingZone.OnRoomEntered += CheckForScareRoom;
    }
    void OnDisable()
    {
        GameEvents.WitchingZone.OnRoomEntered -= CheckForScareRoom;
    }

    void Start()
    {
        dialogueMonsterAI = FindFirstObjectByType<WZDialogueMonsterAI>();
    }

    void CheckForScareRoom(Room room)
    {
        if (room.GetComponent<WZScareRoom>() && !dialogueMonsterAI.hasStarted)
        {
            currentScareRoom = room.GetComponent<WZScareRoom>();
            dialogueMonsterAI.StartChase(currentScareRoom.monsterSpawnPoint.position, currentScareRoom.monsterSpawnPoint.rotation);
        }
        else if (!room.GetComponent<PuzzleBase>())
        {
            dialogueMonsterAI.RoomEntered();
        }
    }
}
