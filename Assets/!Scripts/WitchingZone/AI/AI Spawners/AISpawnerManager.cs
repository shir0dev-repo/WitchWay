using UnityEngine;
using DungeonMaster2D;
using System.Collections.Generic;
using System.Collections;

public class AISpawnerManager : MonoBehaviour
{
    [SerializeField] private List<MonoBehaviour> aiSpawners;

    private void OnEnable()
    {
        GameEvents.WitchingZone.OnDungeonGenerated += SpawnAIsInDungeon;
        GameEvents.WitchingZone.OnRoomEntered += OnRoomEntered;
        GameEvents.WitchingZone.OnRoomExited += OnRoomExited;
    }

    private void OnDisable()
    {
        GameEvents.WitchingZone.OnDungeonGenerated -= SpawnAIsInDungeon;
        GameEvents.WitchingZone.OnRoomEntered -= OnRoomEntered;
        GameEvents.WitchingZone.OnRoomExited -= OnRoomExited;
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

                        StartCoroutine(FindAndAddSpawnedEnemies(room, spawnPoint.position));
                        enemySpawned = true;
                    }
                }
            }
        }
    }

    private IEnumerator FindAndAddSpawnedEnemies(Room room, Vector3 spawnPosition)
    {
        yield return null;

        foreach (var chaseAI in FindObjectsByType<WZChaseAI>(FindObjectsSortMode.None))
        {
            if (Vector3.Distance(chaseAI.transform.position, spawnPosition) < 2f)
            {
                chaseAI.TransitionToState(WZChaseData.State.Inactive);
                room.ChaseEnemies.Add(chaseAI);
                
                chaseAI.transform.SetParent(room.transform);
                break;
            }
        }

        foreach (var mimicAI in FindObjectsByType<WZMimicAI>(FindObjectsSortMode.None))
        {
            if (Vector3.Distance(mimicAI.transform.position, spawnPosition) < 2f)
            {
                room.MimicEnemies.Add(mimicAI);
                
                mimicAI.transform.SetParent(room.transform);
                break;
            }
        }
    }

    private void OnRoomEntered(Room room)
    {
        Debug.Log($"Entering room: {room.name}, ChaseEnemies: {room.ChaseEnemies.Count}, MimicEnemies: {room.MimicEnemies.Count}");
        
        foreach (var chaseAI in room.ChaseEnemies)
        {
            chaseAI.TransitionToState(WZChaseData.State.Idle);
        }
    }

    private void OnRoomExited(Room room, Entrance entrance)
    {
        
        foreach (var chaseAI in room.ChaseEnemies)
        {
            chaseAI.TransitionToState(WZChaseData.State.Inactive);
        }
    }
}