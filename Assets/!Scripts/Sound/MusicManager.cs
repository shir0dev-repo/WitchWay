using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using DungeonMaster2D;
using System.Collections;

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
            case "Shop":
                SoundManager.Instance.PlayMusicTrack(CraftingTrack);
                break;
            case "Witching Zone":
                StartCoroutine(PlayWZTrackCoroutine());
                break;
            case "Menu":
                // MenuTrack
                break;
            default:
                Debug.Log("MusicManager: Unhandled scene loaded: " + scene.name);
                break;
        }
    }

    private IEnumerator PlayWZTrackCoroutine()
    {
        WZPlayerController controller = null;
        do
        {
            controller = WZPlayerController.Instance;
            yield return new WaitForEndOfFrame();
        } while (controller == null);

        yield return new WaitUntil(() => controller.gameObject.activeInHierarchy);

        SoundManager.Instance.PlayMusicTrack(WitchingZoneTrack);
    }
}