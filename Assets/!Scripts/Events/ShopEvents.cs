using System;
using UnityEngine;

public static partial class GameEvents
{
    public static class ShopEvent 
    {
        public static Action OnCustomerEntered;
        public static Action OnCustomerExited;
        public static Action OnPotionSold;
        public static Action OnDialogueStarted;
        public static Action OnDialogueEnded;
    }
}