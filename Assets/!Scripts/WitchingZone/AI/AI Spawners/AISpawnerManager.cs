using UnityEngine;
using DungeonMaster2D;
using System.Collections.Generic;

public class AISpawnerManager : MonoBehaviour
{
    [SerializeField] private List<MonoBehaviour> aiSpawners;

    private void OnEnable()
    {
        GameEvents.WitchingZone.OnDungeonGenerated += SpawnAIsInDungeon;
        GameEvents.WitchingZone.OnRoomEntered += OnRoomEntered;
        GameEvents.WitchingZone.OnRoomExited += OnRoomExited; // Add this line
    }

    private void OnDisable()
    {
        GameEvents.WitchingZone.OnDungeonGenerated -= SpawnAIsInDungeon;
        GameEvents.WitchingZone.OnRoomEntered -= OnRoomEntered;
        GameEvents.WitchingZone.OnRoomExited -= OnRoomExited; // Add this line
    }

    private void SpawnAIsInDungeon(Dungeon2D dungeon)
    {
        foreach (Room room in FindObjectsByType<Room>(FindObjectsSortMode.None))
        {
            if (room.Node == null || room.Node.NodeType == NodeType.Start)
                continue;

            foreach (Transform spawnPoint in room.EnemySpawnPoints)
            {
                bool enemySpawned = false;
                foreach (var spawner in aiSpawners)
                {
                    if (enemySpawned) break;

                    if (spawner is IPercentChanceSpawn aiSpawner && aiSpawner.ShouldSpawn())
                    {
                        aiSpawner.Spawn(spawnPoint.position, spawnPoint.rotation);

                        foreach (var chaseAI in FindObjectsByType<WZChaseAI>(FindObjectsSortMode.None))
                        {
                            if (Vector3.Distance(chaseAI.transform.position, spawnPoint.position) < 0.1f)
                            {
                                chaseAI.TransitionToState(WZChaseData.State.Inactive);
                                room.ChaseEnemies.Add(chaseAI);
                                enemySpawned = true;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    private void OnRoomEntered(Room room)
    {
        Debug.Log($"Entering room: {room.name}, ChaseEnemies: {room.ChaseEnemies.Count}");
        foreach (var chaseAI in room.ChaseEnemies)
        {
            chaseAI.TransitionToState(WZChaseData.State.Idle);
        }
    }

    private void OnRoomExited(Room room)
    {
        Debug.Log($"Exiting room: {room.name}, ChaseEnemies: {room.ChaseEnemies.Count}");
        foreach (var chaseAI in room.ChaseEnemies)
        {
            chaseAI.TransitionToState(WZChaseData.State.Inactive);
        }
    }
}