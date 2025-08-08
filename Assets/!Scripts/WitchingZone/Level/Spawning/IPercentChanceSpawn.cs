using UnityEngine;

public interface IPercentChanceSpawn
{
    float Chance { get; }
    bool ShouldSpawn();
    void Spawn(Vector3 position, Quaternion rotation);
}