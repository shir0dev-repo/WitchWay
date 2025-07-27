using UnityEngine;

public class MimicSpawner : MonoBehaviour, IPercentChanceSpawn
{
    [SerializeField] private GameObject mimicPrefab;
    [SerializeField, Range(0, 1)] private float chance = 0.3f;
    [SerializeField] private Vector3 spawnOffset = Vector3.zero;

    public float Chance => chance;

    public bool ShouldSpawn()
    {
        return Random.value < Chance;
    }

    public void Spawn(Vector3 position, Quaternion rotation)
    {
        Instantiate(mimicPrefab, position + spawnOffset, rotation);
    }
}