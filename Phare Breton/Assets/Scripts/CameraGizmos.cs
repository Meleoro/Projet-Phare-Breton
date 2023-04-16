using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraGizmos : MonoBehaviour
{
    [SerializeField] private Color cameraViewColor;
    [SerializeField] private float rangeCamera;
    private Camera currentCamera;
    
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = cameraViewColor;

        Matrix4x4 tempMatrix = Gizmos.matrix;
            
        if (currentCamera == null)
            currentCamera = ReferenceManager.Instance._camera;

            
        // Camera Pos 1
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawFrustum(Vector3.zero, currentCamera.fieldOfView, rangeCamera, currentCamera.nearClipPlane, currentCamera.aspect);
        

        Gizmos.matrix = tempMatrix;
    }
}
