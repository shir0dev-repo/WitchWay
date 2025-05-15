using UnityEngine;

public class InfinateShelves : MonoBehaviour
{
    [Header("Scroll Settings")]
    [SerializeField] private float scrollSpeed;

    [Header("Loop Settings")]
    [SerializeField] private float shelfHeight; //25 with my test setup

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
            CheckPassedThresehold(trans);
            
            trans.localPosition += delta;
        }
    }

    private void CheckPassedThresehold(Transform objectTrans)
    {
        Transform otherShelf = GetOther(shelfModels, objectTrans);

        //check too high
        if (objectTrans.localPosition.y > shelfHeight + 1)
        {
            objectTrans.localPosition = new Vector3(otherShelf.localPosition.x, otherShelf.localPosition.y - shelfHeight, otherShelf.localPosition.z);
        }
        //check too low
        else if (objectTrans.localPosition.y < -shelfHeight - 1)
        {
            objectTrans.localPosition = new Vector3(otherShelf.localPosition.x, otherShelf.localPosition.y + shelfHeight, otherShelf.localPosition.z);
        }
    }

    //Get other object of pair
    private Transform GetOther(Transform[] pair, Transform current) //can be chnaged ot any type?
    {
        if (pair.Length != 2) return null;

        return pair[0] == current ? pair[1] : pair[0];
    }
}
