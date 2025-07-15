using UnityEngine;

public class CauldronEvents : MonoBehaviour
{
    public delegate void ActivateMixingMode();
    public static ActivateMixingMode ActivateMixing;
    public static ActivateMixingMode DeactivateMixing;
}
