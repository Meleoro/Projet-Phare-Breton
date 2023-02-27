using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCamera : MonoBehaviour
{
    [SerializeField] private BoxCollider _collider;
    [SerializeField] private Transform cameraPos;
    [SerializeField] private Color gizmosColor;


    private void OnTriggerEnter(Collider other)
    {
        
    }


    private void OnDrawGizmos()
    {
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.matrix = rotationMatrix;
        
        Gizmos.color = gizmosColor;
        Gizmos.DrawCube(_collider.center, _collider.size);
    }
}
