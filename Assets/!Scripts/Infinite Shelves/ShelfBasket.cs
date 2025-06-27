using System;
using UnityEngine;

/*
change display if locked or unlocked
*/
[RequireComponent(typeof(Collider2D))]
public class ShelfBasket : MonoBehaviour
{
    [SerializeField] private IngredientSO storedIngredient;

    [Header("Visuals")]
    [SerializeField] private Transform displayPoint;
    [SerializeField] private Vector3 displayScale;
    [SerializeField] private Vector3 displayRotation;

    [Header("Mouse Detection")]
    [SerializeField] private Collider2D detectCollider;
    [SerializeField] private float zGrabPos;

    private bool inBounds = false;
    private bool draggingIngred = false;

    private GameObject displayObject;
    private GameObject grabbedObject;

    void Start()
    {
        SetupDisplayItem();
    }

    void Update()
    {
        CheckMouseInBounds();
        Debug.DrawLine(GetMousePos(), Vector3.forward * 100, Color.red);

        if (inBounds)
        {
            if (Input.GetMouseButtonDown(0))
            {
                GrabIngredient();
            }
            else if (Input.GetMouseButtonUp(0) && draggingIngred)
            {
                ReleaseIngredient(true);
            }
        }
        else
        {
            if (draggingIngred)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    ReleaseIngredient(false);
                }
            }
        }
    }

    private void SetupDisplayItem()
    {
        if (storedIngredient.WorldPrefab != null)
        {
            displayObject = Instantiate(storedIngredient.WorldPrefab, displayPoint.position, Quaternion.identity);
            displayObject.transform.SetParent(displayPoint);
            displayObject.transform.localScale = displayScale;
            displayObject.transform.eulerAngles = displayRotation;
            if (displayObject.GetComponent<WorldIngredient>()) displayObject.GetComponent<WorldIngredient>().enabled = false;
            if (displayObject.GetComponent<Rigidbody>()) displayObject.GetComponent<Rigidbody>().isKinematic = true;
        }
        //fallback for no set visual
        else
        {
            Type[] components = { typeof(MeshFilter), typeof(MeshRenderer) };
            GameObject tempObj = new GameObject(storedIngredient.name, components);
            displayObject = Instantiate(tempObj, displayPoint.position, Quaternion.identity);
            displayObject.transform.SetParent(displayPoint);
            displayObject.transform.localScale = displayScale;
            displayObject.transform.eulerAngles = displayRotation;
            Destroy(tempObj);

            GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Mesh defaultMesh = temp.GetComponent<MeshFilter>().mesh;
            displayObject.GetComponent<MeshFilter>().mesh = defaultMesh;
            Destroy(temp);
        }
    }

    private void CheckMouseInBounds()
    {
        if (CheckBounds2D(detectCollider.bounds, GetMousePos()))
        {
            inBounds = true;
        }
        else
        {
            inBounds = false;
        }
    }

    private void GrabIngredient()
    {
        grabbedObject = Instantiate(displayObject, GetMousePos(), Quaternion.identity);
        Vector3 grabbedTransPos = grabbedObject.transform.position;
        grabbedObject.transform.position = new Vector3(grabbedTransPos.x, grabbedTransPos.y, zGrabPos);
        WorldIngredient wIngred = grabbedObject.GetComponent<WorldIngredient>();
        if (wIngred)
        {
            wIngred.enabled = true; //works once there proper world perfabs
            wIngred._isDragging = true;
            draggingIngred = true;
            wIngred.currentDepth = zGrabPos;

            if (grabbedObject.GetComponent<Rigidbody>())
            {
                grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
                grabbedObject.GetComponent<Rigidbody>().useGravity = false;
            }    
        }
        else
        {
            Debug.LogWarning("NO WORLD PREFAB SELF DESTRUCT");
        }
    }

    private void ReleaseIngredient(bool bounds)
    {
        if (bounds)
        {
            Destroy(grabbedObject); //technically never removed sooo
        }
        else
        {
            //world ingredient script should just drop it into a collider so that it gets brung to stations
        }
    }

    private Vector2 GetMousePos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(Camera.main.transform.position.z);
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    private bool CheckBounds2D(Bounds bounds, Vector2 position)
    {
        if (position.x >= bounds.min.x && position.x <= bounds.max.x &&
        position.y >= bounds.min.y && position.y <= bounds.max.y)
        {
            return true;
        }

        return false;
    }
}
