using UnityEngine;

public class InfiniteShelves : MonoBehaviour
{
    [Header("Momentum Settings")]
    [SerializeField] private float keyAcceleration;
    [SerializeField] private float scrollImpulse;
    [SerializeField] private float scrollFriction;
    [SerializeField] private float dragSens;

    [Header("Loop Settings")]
    [SerializeField] private float shelfHeight; //25 with my test setup
    [SerializeField] private float shelfSpacing; //5 with my test setup

    [Header("Objects")]
    [SerializeField] private Transform[] shelfModels;
    [SerializeField] private Transform[] shelves;
    [SerializeField] private GameObject[] shelfWalls;

    //private vars
    private bool isDragging;
    private Vector3 lastMousePos;
    private LayerMask wallLayer;
    private float scrollVelocity;

    void Start()
    {
        wallLayer = LayerMask.GetMask("ShelfWalls");
    }

    void Update()
    {
        HandleDragInput();
        ReadInputs();
        UpdateShelfPositions();
    }

    private void HandleDragInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                foreach (GameObject wall in shelfWalls) //chnaging the layer makes them invisible so idk
                {
                    if (hit.collider.gameObject == wall)
                    {
                        isDragging = true;
                        lastMousePos = Input.mousePosition;
                        break;
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 delta = Input.mousePosition - lastMousePos;
            scrollVelocity += delta.y * dragSens;
            lastMousePos = Input.mousePosition;
        }
    }

    private void ReadInputs()
    {
        //get input
        float arrow = Input.GetAxis("Vertical");
        scrollVelocity += arrow * keyAcceleration * Time.deltaTime;

        float wheel = Input.GetAxis("Mouse ScrollWheel");
        scrollVelocity += wheel * scrollImpulse;

        scrollVelocity *= 1f / (1f + scrollFriction * Time.deltaTime);
    }

    //Update postions of the shelf models
    private void UpdateShelfPositions()
    {
        Vector3 delta = Vector3.up * (scrollVelocity * Time.deltaTime);

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

        UpdateShelfWalls(delta);
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

    private void UpdateShelfWalls(Vector3 delta)
    {
        foreach (GameObject obj in shelfWalls)
        {
            Renderer rend = obj.GetComponent<Renderer>(); ;
            Material mat = rend.material;
            Vector2 tile = mat.mainTextureScale;
            Vector3 size = rend.bounds.size;

            float factorX = tile.x / size.x;
            float factorY = tile.y / size.y;

            Vector2 uvDelta = new Vector2(delta.x * factorX, delta.y * factorY);

            mat.mainTextureOffset -= uvDelta;
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
        float highestY = float.MinValue;
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
        float lowestY = float.MaxValue;
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
