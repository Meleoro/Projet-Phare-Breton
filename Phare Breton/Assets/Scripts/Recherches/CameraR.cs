using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraR : MonoBehaviour
{
    private Camera _camera;

    [Header("EtatsCamera")] 
    private bool isStatic;
    

    private void Start()
    {
        _camera = GetComponent<Camera>();
    }



    void OrientateCamera(float newXRotation)
    {
        transform.rotation = Quaternion.Euler(45, newXRotation, 0);
    }

    void ChangeZoom(float newZoom)
    {
        _camera.orthographicSize = newZoom;
    }

    void ChangePosXZ(float newX, float newZ)
    {
        transform.position = new Vector3(newX, transform.position.y, newZ);
    }
    
    void ChangePosY(float newY)
    {
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
