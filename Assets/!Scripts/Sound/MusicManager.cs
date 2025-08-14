using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;

public class MusicManager : MonoBehaviour
{
    [Header("Music Tracks")]
    public EventReference CraftingTrack, WitchingZoneTrack, MenuTrack;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SoundManager.Instance.StopMusic();
        switch (scene.name)
        {
            case "Crafting":
                SoundManager.Instance.PlayMusicTrack(CraftingTrack);
                break;
            case "WitchingZone":
                // WitchingZoneTrack
                break;
            case "Menu":
                // MenuTrack
                break;
            default:
                Debug.Log("MusicManager: Unhandled scene loaded: " + scene.name);
                break;
        }
    }
}