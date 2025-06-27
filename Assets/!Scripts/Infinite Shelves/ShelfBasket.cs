using System;
using UnityEngine;

/*
display ingrediant
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

    private bool inBounds = false;

    private GameObject displayObject;

    void Start()
    {
        SetupDisplayItem();
    }

    void Update()
    {
        CheckMouseInBounds();
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
        if (detectCollider.bounds.Contains(GetMousePos()))
        {
            inBounds = true;
        }
        else
        {
            inBounds = false;
        }
    }

    private Vector2 GetMousePos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(Camera.main.transform.position.z);
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
}
