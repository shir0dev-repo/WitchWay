using UnityEngine;
using UnityEngine.SceneManagement;

public class WZExitPortal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out WZPlayerController controller))
        {
            if (ShopManager.Instance != null)
            {
                ShopManager.Instance.UnloadArea();
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetSceneByName("Shop").buildIndex);
            }
        }
    }
}
