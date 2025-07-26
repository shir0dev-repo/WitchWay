using DungeonMaster2D;
using System;
using UnityEngine;

public static partial class GameEvents
{
    public static class WitchingZone
    {
        public static Action<Vector3> OnIngredientPickedUp;
        public static Action OnSanityChanged;
        public static Action OnSanityIncreased;
        public static Action OnSanityDecreased;

        public static Action<Dungeon2D> OnDungeonGenerated;

        public static Action<Room> OnRoomExited;
        public static Action<Room> OnRoomEntered;

        public static Action OnDoorUnlocked;
    }
}
