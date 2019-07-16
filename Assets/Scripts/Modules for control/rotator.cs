﻿/// A general class that is used as a method for rotating the end-effector
/// based on the use of visual reference points for a gameobject
/// Created on July 3, 2019 by Esteban Segarra 


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotator : MonoBehaviour { 
    private Vector3 mOffset;
    private float mZCoord;

    Quaternion init_quaternion;
    
    //Initialize to locate the starting angle
    void Start()
    {
        init_quaternion = transform.localRotation; 
    }

    //Find the reference point on the gameobject and set to it. 
    private void OnMouseDown()
    {
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        mOffset = gameObject.transform.position - GetMouseWorldPos();
    }

    //Convert the mouse pointer location from the screen to the virtual world
    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    Vector3 targetPosition;
    Ray cameraRay;              // The ray that is cast from the camera to the mouse position
    RaycastHit cameraRayHit;    // The object that the ray hits

    public Vector3 delta = Vector3.zero;
    private Vector3 lastPos = Vector3.zero;

    void Update()
    {
        //transform.localRotation = init_quaternion;


        //This section allows the program to determine the raw delta values for the mouse position changes
        if (Input.GetMouseButtonDown(0))
        {
            lastPos = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            delta = Input.mousePosition - lastPos;
            // End do stuff
            lastPos = Input.mousePosition;
        }
    }



    private void OnMouseDrag()
    {
        Vector3 mouse_world_pos = GetMouseWorldPos();
        Vector3 new_pos;
        Vector3 mouse_pos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, mZCoord);

        //Depending on what gameobject this item is applied, the behaviour is dependent on the 
        //object name applied. 
        switch (this.name)
        {
            //Allows freeform movement in the world
            case "Control_pt":
                transform.parent.position = mouse_world_pos + mOffset;
                break;
            //Y axis transformations 
            case "Y_axis":
                new_pos = Camera.main.ScreenToWorldPoint(mouse_pos);
                transform.parent.position = new Vector3(transform.root.position.x, new_pos.y, transform.root.position.z);
                break;

                //The Z-axis is a complicated matter as the user only has X and Y components. This one is faked by creating delta X and Y components 
            case "X_axis":
                if (double.IsNaN(delta.y / Mathf.Abs(delta.y)))
                {
                    break;
                }
                else if ((delta.x / Mathf.Abs(delta.x)) != 0)
                {
                    transform.root.position = new Vector3(transform.root.position.x, transform.root.position.y, transform.root.position.z + -(delta.y / Mathf.Abs(delta.y)) * 0.2F);

                    break;
                } 

                if (double.IsNaN(delta.x / Mathf.Abs(delta.x)))
                {
                    break;
                }else if ((delta.x / Mathf.Abs(delta.x)) != 0)
                {
                    transform.root.position = new Vector3(transform.root.position.x, transform.root.position.y, transform.root.position.z + -(delta.x / Mathf.Abs(delta.x)) * 0.2F);

                    break;
                }
                break;
            //Case for when the user desires to move in the Z unity axis 
            case "Z_axis":
                new_pos = Camera.main.ScreenToWorldPoint(mouse_pos);
                transform.root.position = new Vector3(new_pos.x, transform.root.position.y, transform.root.position.z);
                break;
            //Allows rotation in the Y-axis 
            case "Y_rot_axis":
                if (double.IsNaN(delta.y / Mathf.Abs(delta.y)))
                {
                    break;
                }
                Vector3 temp = new Vector3(transform.root.eulerAngles.x, transform.root.eulerAngles.y + (delta.y / Mathf.Abs(delta.y)), transform.root.eulerAngles.z);
                transform.root.eulerAngles = temp;
                
                break;
            //Allows rotation in the Z axis 
            case "Z_rot_axis":
                if (double.IsNaN(delta.x / Mathf.Abs(delta.x)))
                {
                    break;
                }
                transform.root.eulerAngles = new Vector3(transform.root.eulerAngles.x, transform.root.eulerAngles.y, transform.root.eulerAngles.z + (delta.x / Mathf.Abs(delta.x)));
                break;

            //Allows rotation in the X axis 
            case "X_rot_axis":
                if (double.IsNaN(delta.y / Mathf.Abs(delta.y)))
                {
                    break;
                }
                transform.root.eulerAngles = new Vector3(transform.root.eulerAngles.x + delta.y / Mathf.Abs(delta.y), transform.root.eulerAngles.y, transform.root.eulerAngles.z);
                break;
            default:
                break;
        }
    }
}
