using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private List<SoundSO> sounds;

    private FMODUnity.StudioEventEmitter _emitter;

    void Start()
    {
        _emitter = gameObject.AddComponent<FMODUnity.StudioEventEmitter>();
        foreach (var sound in sounds)
        {
            string evtName = sound.EventName;

            FieldInfo targetEvent = 
                typeof(GameEvents.WitchingZone).GetField(evtName) ??
                typeof(GameEvents.Crafting).GetField(evtName);


            if (targetEvent == null)
            {
                Debug.LogWarning($"Unknown event name: {sound.EventName} in both WitchingZone and Crafting");
                continue;
            }

            Type eventSignature = targetEvent.FieldType;
            if (!typeof(Delegate).IsAssignableFrom(eventSignature))
                throw new InvalidOperationException($"Specified field `{eventSignature.FullName} is not of type Delegate!");
            Debug.Log(eventSignature.ToString());

            // Break down invoked event from whatever signature it has
            // i.e. Action<T1, T2, TN> is deconstructed into Delegate object.
            Delegate wrapper = BuildDelegate(
                () =>
                {
                    PlaySound(sound);
                    Debug.Log(sound.EventName);
                }, eventSignature);

            // Append the new delegate to the existing call list
            targetEvent.SetValue(null, Delegate.Combine((Delegate)targetEvent.GetValue(null), wrapper));
        }
    }

    void PlaySound(SoundSO sound)
    {
        if (sound != null && !sound.FMODEventPlayable.IsNull)
        {
            _emitter.EventReference = sound.FMODEventPlayable;
            _emitter.Play();
        }
    }

    private static Delegate BuildDelegate(Action method, Type delegateType)
    {
        MethodInfo invoke = delegateType.GetMethod("Invoke")!;
        var paramExprs = invoke
            .GetParameters()
            .Select(p => Expression.Parameter(p.ParameterType, p.Name ?? "_"))
            .ToArray();

        var body = Expression.Invoke(Expression.Constant(method));
        var lambda = Expression.Lambda(delegateType, body, paramExprs);

        return lambda.Compile();
    }
}
