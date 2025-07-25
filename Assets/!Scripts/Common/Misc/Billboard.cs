using UnityEngine;

public class Billboard : MonoBehaviour
{
    private static Camera _mainCamera;

    private void Awake()
    {
        if (_mainCamera == null)
            _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (_mainCamera != null)
        {
            transform.LookAt(-_mainCamera.transform.position);
        }
    }
}
