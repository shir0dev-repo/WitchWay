using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CloseAdditiveSceneButton : MonoBehaviour
{
    private Button _btn;

    private void Awake()
    {
        _btn = GetComponent<Button>();
        _btn.onClick.AddListener(ShopManager.Instance.UnloadArea);
    }
}