using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DebugMenuManager : MonoBehaviour
{
    public static DebugMenuManager Instance { get; private set; }
    bool isDebugMenuActive = false;
    [SerializeField] TMP_Text debugMenuButton;
    [SerializeField] RectTransform debugMenuPanel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void ToggleDebugMenu()
    {
        if (isDebugMenuActive)
        {
            isDebugMenuActive = false;
            debugMenuPanel.position = new Vector3(-debugMenuPanel.position.x, debugMenuPanel.position.y, debugMenuPanel.position.z);
            debugMenuButton.text = "-->";
        }
        else
        {
            isDebugMenuActive = true;
            debugMenuPanel.position = new Vector3(-debugMenuPanel.position.x, debugMenuPanel.position.y, debugMenuPanel.position.z);
            debugMenuButton.text = "<--";
        }
    }
    
    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
