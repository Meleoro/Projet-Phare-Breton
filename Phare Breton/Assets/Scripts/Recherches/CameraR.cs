using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraR : MonoBehaviour
{
    private Camera _camera;
    
    [Header("TransitionGeneral")]
    private bool isStatic;
    private float currentAvancee;
    
    [Header("PosTransition")] 
    public Vector3 startPos;
    public Vector3 midPos;
    public Vector3 endPos;
    private Vector3 currentPos;
    
    [Header("ZoomTransition")] 
    public float startZoom;
    public float midZoom;
    public float endZoom;
    private Vector3 currentZoom;
    

    private void Start()
    {
        _camera = GetComponent<Camera>();

        isStatic = true;
    }


    private void Update()
    {
        if (!isStatic)
        {
            
        }
    }


    public void TransitionCamera(Vector3 start, Vector3 end)
    {
        float distanceStart = Vector3.Distance(startPos, startPos);
    }
    

    public void OrientateCamera(float newXRotation)
    {
        transform.rotation = Quaternion.Euler(45, newXRotation, 0);
    }

    public void ChangeZoom(float newZoom)
    {
        _camera.orthographicSize = newZoom;
    }

    public void ChangePosXZ(float newX, float newZ)
    {
        transform.position = new Vector3(newX, transform.position.y, newZ);
    }
    
    public void ChangePosY(float newY)
    {
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
