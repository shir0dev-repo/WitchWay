using UnityEngine;
using UnityEngine.EventSystems;

public class WorldIngredient : MonoBehaviour
{
    public IngredientSO ingredient; //added this so can ref what ingrediant it is

    [HideInInspector] public bool _isDragging = false;

    private Vector3 _mousePosWS = Vector3.zero;
    private Vector3 _velocity = Vector3.zero;
    private float _moveSpeed = 4.5f;

    private static Camera _cam = null;

    private void Start()
    {
        if (_cam == null)
            _cam = Camera.main;
        //GetComponent<MeshRenderer>().material.color = Random.ColorHSV(0, 1, 1, 1, 1, 1, 1, 1);
    }

    private void Update()
    {
        if (!_isDragging) return;

        transform.position = Vector3.SmoothDamp(transform.position, _mousePosWS, ref _velocity, _moveSpeed * Time.deltaTime);
    }

    public void OnMouseDown()
    {
        Debug.Log("clicked");
        _mousePosWS = GetMousePos();
        _isDragging = true;
    }

    public void OnMouseDrag()
    {
        _mousePosWS = GetMousePos();
    }

    public void OnMouseUp()
    {
        _isDragging = false;
    }

    private Vector3 GetMousePos()
    {
        Vector3 pos = Input.mousePosition - _mousePosWS;
        Vector3 oProjC = Vector3.Project(transform.position - _cam.transform.position, _cam.transform.forward);
        pos.z = oProjC.magnitude;

        Vector3 re = _cam.ScreenToWorldPoint(pos);

        return new Vector3(re.x, re.y, gameObject.transform.position.z);
    }
}
