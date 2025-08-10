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
            dialogueMonsterAI.StartChase(currentScareRoom.monsterSpawnPoint.position+new Vector3(0,1.3f,0), currentScareRoom.monsterSpawnPoint.rotation);
        }
        else
        {
            dialogueMonsterAI.RoomEntered();
        }
    }
}
