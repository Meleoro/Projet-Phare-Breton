using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneJump : MonoBehaviour
{
    [SerializeField] private BoxCollider _collider;
    [SerializeField] private Color gizmoColor;




    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {

        }
    }



    private void OnDrawGizmos()
    {
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.matrix = rotationMatrix;

        Gizmos.color = gizmoColor;
        Gizmos.DrawCube(_collider.center, _collider.size);
    }
}
