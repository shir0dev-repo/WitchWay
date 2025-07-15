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
