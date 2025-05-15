using Unity.VisualScripting;
using UnityEngine;

public class InfinateShelves : MonoBehaviour
{
    [Header("Scroll Settings")]
    [SerializeField] private float scrollSpeed;

    [Header("Loop Settings")]
    [SerializeField] private float shelfHeight; //25 with my test setup
    [SerializeField] private float shelfSpacing; //5 with my test setup

    [Header("Objects")]
    [SerializeField] private Transform[] shelfModels;
    [SerializeField] private Transform[] shelves;

    //private vars
    float verticalInput;

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

    //Update postions of the shelf models
    private void UpdateShelfPositions()
    {
        Vector3 delta = Vector3.up * (verticalInput * scrollSpeed * Time.deltaTime);

        //updates the shelves (parent objects for things sitting on shelves)
        foreach (Transform trans in shelves)
        {
            CheckShelfPassedThreshold(trans);

            trans.localPosition += delta;
        }

        //update models
        foreach (Transform trans in shelfModels)
        {
            CheckPassedThreshold(trans);

            trans.localPosition += delta;
        }
    }

    //check if model is too far down or up
    private void CheckPassedThreshold(Transform objectTrans)
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

    //check if sheleves are too far down or up
    private void CheckShelfPassedThreshold(Transform objectTrans)
    {
        //check too high
        if (objectTrans.localPosition.y > (shelfHeight / 2) + 1)
        {
            objectTrans.localPosition = new Vector3(objectTrans.localPosition.x, GetLowestShelf().localPosition.y - shelfSpacing, objectTrans.localPosition.z);
        }
        //check too low
        else if (objectTrans.localPosition.y < -(shelfHeight / 2) - 1)
        {
            objectTrans.localPosition = new Vector3(objectTrans.localPosition.x, GetHighestShelf().localPosition.y + shelfSpacing, objectTrans.localPosition.z);
        }
    }

    //Get other object of pair (for shelves)
    private Transform GetOther(Transform[] pair, Transform current) //can be chnaged ot any type?
    {
        if (pair.Length != 2) return null;

        return pair[0] == current ? pair[1] : pair[0];
    }

    private Transform GetHighestShelf()
    {
        float highestY = -5;
        Transform highestShelf = null;

        foreach (Transform shelf in shelves)
        {
            if (shelf.localPosition.y > highestY)
            {
                highestShelf = shelf;
                highestY = shelf.localPosition.y;
            }
        }

        return highestShelf;
    }

    private Transform GetLowestShelf()
    {
        float lowestY = 5;
        Transform lowestShelf = null;

        foreach (Transform shelf in shelves)
        {
            if (shelf.localPosition.y < lowestY)
            {
                lowestShelf = shelf;
                lowestY = shelf.localPosition.y;
            }
        }

        return lowestShelf;
    }
}
