using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwapTest : MonoBehaviour
{
    [SerializeField] private string sceneName;

    public void SwitchScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
