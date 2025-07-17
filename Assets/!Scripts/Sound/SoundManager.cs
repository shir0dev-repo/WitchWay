using FMODUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private StudioEventEmitter _emitterSFX;
    [SerializeField] private StudioEventEmitter _emitterMusic;

    void Update()
    {
        // Delete this later, I just didnt want to constantly hear the music
        if (Input.GetKeyDown(KeyCode.F5))
        {
            _emitterMusic.Stop();
        }
    }
    public void PlayMusicTrack(EventReference track)
    {
        _emitterMusic.EventReference = track;
        _emitterMusic.Play();
    }

    public void PlayOneShot(EventReference sound, Vector3 position = default)
    {
        try
        {
            _emitterSFX.EventReference = sound;
            RuntimeManager.PlayOneShot(sound, position);
        }
        catch (Exception e)
        {
            Debug.LogError($"SoundManager: Failed to play sound {sound} at position {position}. Error: {e.Message}");
            return;
        }

    }
}
