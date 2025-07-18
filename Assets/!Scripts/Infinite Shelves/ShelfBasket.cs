using System;
using UnityEngine;

/*
change display if locked or unlocked
*/
[RequireComponent(typeof(Collider2D))]
public class ShelfBasket : MonoBehaviour
{
    [SerializeField] private IngredientSO storedIngredient;
    [SerializeField] private bool _isUnlocked = false; //intended as temp until proper unlocks system

    //monitor the bool during runtime
    public bool IsUnlocked
    {
        get => _isUnlocked;
        set
        {
            if (_isUnlocked != value)
            {
                _isUnlocked = value;
                SetupUnlockVisual();
            }
        }
    }

    [Header("Visuals")]
    [SerializeField] private Transform displayPoint;
    [SerializeField] private Vector3 displayRotation;
    [SerializeField] private Material lockedMaterial;

    [Header("Mouse Detection")]
    [SerializeField] private Collider2D detectCollider;
    [SerializeField] private float zGrabPos;

    private bool inBounds = false;
    private bool draggingIngred = false;

    private GameObject displayObject;
    private Material displayObjDefaultMat;
    private GameObject grabbedObject;

    void Start()
    {
        SetupDisplayItem();
    }

#if UNITY_EDITOR
    //when editing inspetor value
    private void OnValidate()
    {
        if (displayObject != null) SetupUnlockVisual();
    }
#endif

    void Update()
    {
        CheckMouseInBounds();
        Debug.DrawLine(GetMousePos(), Vector3.forward * 100, Color.red);

        if (inBounds && IsUnlocked)
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
            displayObject.transform.eulerAngles = displayRotation;

            if (displayObject.TryGetComponent(out WorldIngredient wIng)) Destroy(wIng);
            if (displayObject.GetComponent<Rigidbody>()) displayObject.GetComponent<Rigidbody>().isKinematic = true;
        }
        //fallback for no set visual
        else
        {
            Type[] components = { typeof(MeshFilter), typeof(MeshRenderer) };
            GameObject tempObj = new GameObject(storedIngredient.name, components);
            displayObject = Instantiate(tempObj, displayPoint.position, Quaternion.identity);
            displayObject.transform.SetParent(displayPoint);
            displayObject.transform.eulerAngles = displayRotation;
            Destroy(tempObj);

            GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Mesh defaultMesh = temp.GetComponent<MeshFilter>().mesh;
            Material defaultMat = temp.GetComponent<MeshRenderer>().material;
            displayObject.GetComponent<MeshFilter>().mesh = defaultMesh;
            displayObject.GetComponent<MeshRenderer>().material = defaultMat;
            displayObjDefaultMat = defaultMat;
            Destroy(temp);

            print("fallback item create");
        }

        SetupUnlockVisual();
    }

    private void SetupUnlockVisual()
    {
        IsUnlocked = SaveManager.Instance.hasIngredient(storedIngredient.ID.ToString());
        
        if (!IsUnlocked)
        {
            //could move these into function
            if (displayObject.TryGetComponent(out MeshRenderer mr))
            {
                displayObjDefaultMat = mr.material;
                mr.material = null; //idk just temp
            }
            else if (mr = displayObject.GetComponentInChildren<MeshRenderer>())
            {
                displayObjDefaultMat = mr.material;
                mr.material = lockedMaterial;
            }
            else
            {
                Debug.LogWarning("Somehow no material found");
            }
        }
        else
        {
            if (displayObject.TryGetComponent(out MeshRenderer mr))
            {
                displayObjDefaultMat ??= mr.material;
                mr.material = displayObjDefaultMat; //idk just temp
                mr.gameObject.layer = LayerMask.NameToLayer("Default");
            }
            else if (mr = displayObject.GetComponentInChildren<MeshRenderer>())
            {
                displayObjDefaultMat ??= mr.material;
                mr.material = displayObjDefaultMat;
                mr.gameObject.layer = LayerMask.NameToLayer("Default");
            }
            else
            {
                Debug.LogWarning("Somehow no material found");
            }
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
        grabbedObject = Instantiate(storedIngredient.WorldPrefab, GetMousePos(), Quaternion.identity);
        //Vector3 grabbedTransPos = grabbedObject.transform.position;
        //grabbedObject.transform.position = new Vector3(grabbedTransPos.x, grabbedTransPos.y, zGrabPos);
        WorldIngredient wIngred = grabbedObject.GetComponent<WorldIngredient>();
        if (wIngred)
        {
            wIngred.currentDepth = zGrabPos;

            if (grabbedObject.GetComponent<Rigidbody>())
            {
                grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
                grabbedObject.GetComponent<Rigidbody>().useGravity = false;
            }

            if (CursorManager.Instance != null) CursorManager.Instance.AttachToCursor(wIngred, grabbedObject.transform);
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
            //currently all inventory is one so once all flow need to designate between whats stored and what brung to stations
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
