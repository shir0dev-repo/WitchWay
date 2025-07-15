using UnityEngine;

public class CauldronMixingDuration : MonoBehaviour
{
    public float maxDuration = 5;
    [SerializeField] float currDuration = 0;
    public void UpdateCurrentDuration()
    {
        currDuration += Time.deltaTime;

        if (currDuration >= maxDuration)
        {
            CauldronEvents.DeactivateMixing?.Invoke();
            currDuration = 0;
        }
    }
}
