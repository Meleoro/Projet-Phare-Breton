using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneGravier : MonoBehaviour
{
    [Header("Références")]
    public BoxCollider _collider;

    [Header("Gizmos")]
    public Color gizmosColor;


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !other.isTrigger)
        {
            ReferenceManager.Instance.characterReference.movementScript.isOnGravier = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            ReferenceManager.Instance.characterReference.movementScript.isOnGravier = false;
        }
    }


    private void OnDrawGizmos()
    {
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.matrix = rotationMatrix;

        Gizmos.color = gizmosColor;
        Gizmos.DrawCube(_collider.center, _collider.size);
    }
}
