using System;
using UnityEngine;
using FMOD;
using FMODUnity;

[CreateAssetMenu(fileName = "SoundSO", menuName = "Sound/SoundSO")]
public class SoundSO : ScriptableObject
{
    public string EventName;
    public EventReference FMODEventPlayable;
    public AudioClip Clip;
}
