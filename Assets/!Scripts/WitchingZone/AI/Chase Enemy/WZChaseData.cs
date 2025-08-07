using UnityEngine;

[System.Serializable]
public struct WZChaseData 
{
    [Header("State Data")]
    public bool IsWalking;
    public bool IsStopping;
    public bool IsBreathing;
    public bool IsGrunting;
    public enum State
    {
        Inactive,
        Idle,
        Patrol,
        Attack
    }
    public State currentState;

    public readonly Vector3 StartPosition;
    public readonly Quaternion StartRotation;

    public WZChaseData(Vector3 startPos, Quaternion startRot)
    {
        StartPosition = startPos;
        StartRotation = startRot;

        IsWalking = false;
        IsStopping = false;
        IsGrunting = false;
        IsBreathing = false;

        currentState = State.Inactive;
    }
}
