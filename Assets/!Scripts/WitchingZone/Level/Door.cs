using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private float _entryPositionOffset = 1.5f;
    
    public Room AttachedRoom { get; set; } = null;

    public bool IsLocked { get; private set; } = false;

    public void Lock() => IsLocked = true;
    public void Unlock() => IsLocked = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (IsLocked) return;
        if (AttachedRoom == null) return;
        
        if (!collision.gameObject.TryGetComponent(out WZPlayerController controller)) return;
        controller.SetCanMove(false);
        GameEvents.WitchingZone.OnRoomExited?.Invoke(AttachedRoom);
    }
}
