using UnityEngine;

public class InfinateShelves : MonoBehaviour
{
    [Header("Scroll Settings")]
    [SerializeField] private float scrollSpeed;

    [Header("Objects")]
    [SerializeField] private Transform[] shelfModels;

    //private vars
    float verticalInput;

    void Start()
    {

    }

    void Update()
    {
        ScrollInput();

        UpdateShelfPositions();
    }

    private void ScrollInput()
    {
        //get input
        verticalInput = Input.GetAxis("Vertical");
        verticalInput += Input.GetAxis("Mouse ScrollWheel") * 100;
    }

    private void UpdateShelfPositions()
    {
        Vector3 delta = Vector3.up * (verticalInput * scrollSpeed * Time.deltaTime);

        foreach (Transform trans in shelfModels)
        {
            trans.localPosition += delta;
        }
    }
}
