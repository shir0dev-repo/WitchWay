using UnityEngine;

[System.Serializable]
public struct WZMimicData
{
    [Header("State Data")]
    public bool IsActive;
    public bool IsActivating;
    public bool IsDeactivating;
    public bool IsJumping;
    public bool IsGrowling;
    public bool IsTwitching;
    public bool IsAttacking;
    public bool IsBeingLookedAt;

    public readonly Vector3 StartPosition;
    public readonly Quaternion StartRotation;

    public WZMimicData(Vector3 startPos, Quaternion startRot)
    {
        StartPosition = startPos;
        StartRotation = startRot;

        IsActive = false;
        IsActivating = false;
        IsDeactivating = false;
        IsJumping = false;
        IsGrowling = false;
        IsTwitching = false;
        IsAttacking = false;
        IsBeingLookedAt = false;
    }
}
