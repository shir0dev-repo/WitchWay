using FMODUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    private StudioEventEmitter _emitterSFX;
    private StudioEventEmitter _emitterMusic;

    void Start()
    {
        _emitterSFX = gameObject.AddComponent<StudioEventEmitter>();
        _emitterMusic = gameObject.AddComponent<StudioEventEmitter>();
    }

    public void PlayMusicTrack(EventReference track)
    {
        _emitterMusic.EventReference = track;
        _emitterMusic.Play();
    }

    public void PlayOneShot(EventReference sound, Vector3 position = default)
    {
        _emitterSFX.EventReference = sound;
        _emitterSFX.Play();
    }
}
