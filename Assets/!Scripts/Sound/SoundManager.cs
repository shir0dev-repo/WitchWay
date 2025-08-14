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

    private Dictionary<string, FMOD.Studio.EventInstance> _loopingInstances = new Dictionary<string, FMOD.Studio.EventInstance>();
    private FMOD.Studio.EventInstance _currentMusicInstance;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }


    public void PlayMusicTrack(EventReference track)
    {
        StopMusic();

        _currentMusicInstance = RuntimeManager.CreateInstance(track);
        _currentMusicInstance.start();
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
    public void PlayOneShotWithParameter(EventReference sound, string parameterName, float parameterValue, Vector3 position = default)
    {
        var instance = FMODUnity.RuntimeManager.CreateInstance(sound);
        instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(position));
        instance.setParameterByName(parameterName, parameterValue);
        instance.start();
        instance.release();
    }

    public void PlayLoop(string key, EventReference sound, Vector3 position = default)
    {
        if (sound.IsNull)
        {
            Debug.LogWarning("Tried to play null FMODEventReference!");
            return;
        }

        if (_loopingInstances.TryGetValue(key, out var existingInstance))
        {
            existingInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            existingInstance.release();
            _loopingInstances.Remove(key);
        }

        var instance = RuntimeManager.CreateInstance(sound);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
        instance.start();
        _loopingInstances[key] = instance;
    }

    public void StopLoop(string key)
    {
        if (_loopingInstances.TryGetValue(key, out var instance))
        {
            instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            instance.release();
            _loopingInstances.Remove(key);
        }
    }

    public void SetParameterByName(string key, string parameterName, float value)
    {
        if (_loopingInstances.TryGetValue(key, out var instance))
        {
            instance.setParameterByName(parameterName, value);
        }
    }

    public bool IsLooping(string key)
    {
        return _loopingInstances.ContainsKey(key);
    }

    public void StopMusic()
    {
        if (_emitterMusic.IsPlaying())
        {
            _emitterMusic.Stop();
        }
        
        if (_currentMusicInstance.isValid())
        {
            _currentMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            _currentMusicInstance.release();
        }
    }
}
