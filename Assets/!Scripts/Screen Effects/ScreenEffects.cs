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

    private Volume _volume;

    private void Start()
    {
        _volume = GetComponent<Volume>();
    }

    public void DoScreenEffect(string effectName, float duration, float targetWeight, Action callback, bool resetOnFinish, bool forced = false)
    {
        EffectData effect = _effectProfiles.Find(e => e.name == effectName);
        if (effect == null)
        {
            callback();
            return;
        }

        VolumeProfile v = effect.Profile;
        
        _volume.profile = effect.Profile;

        var tween = DOTween.To(() => _volume.weight, x => _volume.weight = x, targetWeight, duration);
        TweenCallback onComplete;
        if (resetOnFinish)
            onComplete = () =>
            {
                callback();
                _volume.weight = 1.0f;
            };
        else onComplete = () => callback();

        tween.onComplete = onComplete;
    }
}
