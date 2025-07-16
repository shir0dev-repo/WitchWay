using System;
using UnityEngine;

public static partial class GameEvents
{
    public static class WitchingZone
    {
        public static Action OnSanityChanged;
        public static Action OnSanityIncreased;
        public static Action OnSanityDecreased;
        public static Action OnDoorUnlocked;

        public static Action OnFishingRodInteractedWith;
        public static Action OnJumpscareRequested;
    }
}
