using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;

    [Header("References")]
    private int mainSceneIndex = 1;  // I can come back later and use the actual name of the scene instead of indexing

    public void StartButton()
    {
        SceneManager.LoadScene(mainSceneIndex);
    }
    public void QuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit()
        #endif
    }
}
