using UnityEngine;

public class SwitchToMixing : MonoBehaviour
{   
    public GameObject AddingIngredientsObjects;
    public GameObject canvas;
    public Camera cam;
    public Transform camTarget;

    public void SwitchToMixNow()
    {
        AddingIngredientsObjects.SetActive(false);
        canvas.SetActive(true);
        cam.transform.rotation = camTarget.transform.rotation;
        cam.transform.position = camTarget.transform.position;
    }
}
