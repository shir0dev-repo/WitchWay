using UnityEngine;

public class SwitchToMixing : MonoBehaviour
{
    public delegate void ActivateMixingMode();
    public static ActivateMixingMode mixingMode;
}
