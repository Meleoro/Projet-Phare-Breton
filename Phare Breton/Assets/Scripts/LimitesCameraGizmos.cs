using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitesCameraGizmos : MonoBehaviour
{
    [SerializeField] private bool desactivateGizmos;
    
    [SerializeField] private Color areaCameraColor;

    [SerializeField] private Transform minXZ;
    [SerializeField] private Transform maxXZ;

    private void OnDrawGizmosSelected()
    {
        if (!desactivateGizmos)
        {
            Gizmos.color = areaCameraColor;

            Vector3 max = minXZ.transform.InverseTransformPoint(maxXZ.position);
                
            Vector3 point1 = new Vector3(0, 0, max.z);
            Vector3 point2 = new Vector3(max.x, max.y, 0);

            point1 = minXZ.transform.TransformPoint(point1);
            point2 = minXZ.transform.TransformPoint(point2);

            Gizmos.DrawLine(minXZ.position, point1);
            Gizmos.DrawLine(minXZ.position, point2);
            Gizmos.DrawLine(maxXZ.position, point1);
            Gizmos.DrawLine(maxXZ.position, point2);
        }
    }
}
