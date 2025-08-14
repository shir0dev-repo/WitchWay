using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;

    // [Header("References")]
    // private int mainSceneIndex = 1;  // I can come back later and use the actual name of the scene instead of indexing

    public void NewGameButton()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.ResetSave();
        }
        SceneManager.LoadScene("Shop");
    }
    public void LoadButton()
    {
        SceneManager.LoadScene("Shop");     // change this to whatever the first tscene the player should see when hitting the play button from the main menu
    }
    public void QuitButton()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveGame();
        }
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // so this should write the saved data to the disc when leaving the witching zone, currently Im going to set it to the crafting scene again will have to change depedning on the structure we want for traversing between scenes
    public void SaveAndLoadScene()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveGame();
        }
        SceneManager.LoadScene("Crafting");
    }

    // dev scene switching
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SaveAndLoadScene();
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            SceneManager.LoadScene("WZPlayerController");
        }
    }
}
