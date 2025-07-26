using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;
using System.Linq;

public class ScreenEffects : Singleton<ScreenEffects>
{
    [System.Serializable]
    public class EffectData
    {
        public VolumeProfile Profile;
        public string name;
    }

    [Header("Profiles")]
    [SerializeField] private List<EffectData> _effectProfiles;
    
    private List<Volume> _volumes = new();

    public void DoScreenEffect(string effectName, float duration, float targetWeight, Action callback, bool resetOnFinish, bool forced = false)
    {
        EffectData effect = _effectProfiles.Find(e => e.name == effectName);
        if (effect == null) return;

        VolumeProfile v = effect.Profile;
        Volume readyVolume = _volumes.FirstOrDefault(vol => vol.weight == 0);
        if (readyVolume == null)
        {
            readyVolume = gameObject.AddComponent<Volume>();
            _volumes.Add(readyVolume);
        }

        var tween = DOTween.To(() => readyVolume.weight, x => readyVolume.weight = x, targetWeight, duration);
        TweenCallback onComplete;
        if (resetOnFinish)
            onComplete = () =>
            {
                callback();
                readyVolume.weight = 0.0f;
            };
        else onComplete = () => callback();

        tween.onComplete = onComplete;
    }
}
