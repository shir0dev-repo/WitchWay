using System;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    public void ObjectFollowsMouse(Rigidbody rb)
    {
        Vector3 mousePos = Input.mousePosition; //get the position of the mouse

        mousePos.z = Math.Abs(Camera.main.transform.position.z - rb.position.z); //get how far the object is from the camera on z axis
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos); 

        rb.position = new Vector3(worldMousePos.x, worldMousePos.y, rb.position.z); //set new position (keeping the object's z axis)
    }
}
