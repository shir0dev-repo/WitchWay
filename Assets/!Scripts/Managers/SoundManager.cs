using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private List<SoundSO> sounds;
    void Start()
    {
        foreach (var sound in sounds)
        {
            bool found = false;

            var witchingZoneField = typeof(GameEvents.WitchingZone).GetField(sound.EventName);
            if (witchingZoneField != null)
            {
            var gameEvent = witchingZoneField.GetValue(null) as System.Action;
            System.Action handler = () => PlaySound(sound);
            witchingZoneField.SetValue(null, (System.Action)System.Delegate.Combine(gameEvent, handler));
            found = true;
            }
            else
            {
            var craftingField = typeof(GameEvents.Crafting).GetField(sound.EventName);
            if (craftingField != null)
            {
                var gameEvent = craftingField.GetValue(null) as System.Action;
                System.Action handler = () => PlaySound(sound);
                craftingField.SetValue(null, (System.Action)System.Delegate.Combine(gameEvent, handler));
                found = true;
            }
            }

            if (!found)
            {
            Debug.LogWarning($"Unknown event name: {sound.EventName} in both WitchingZone and Crafting");
            }
        }
    }

    void PlaySound(SoundSO sound)
    {
        if (sound != null && sound.Clip != null)
        {
            AudioSource.PlayClipAtPoint(sound.Clip, Camera.main.transform.position);
        }
    }
}
