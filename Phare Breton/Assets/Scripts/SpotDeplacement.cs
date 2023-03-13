using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotDeplacement : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.matrix = rotationMatrix;
        
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactible") && !other.isTrigger)
        {
            other.GetComponent<ObjetInteractible>().Magnet(transform);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactible") && !other.isTrigger)
        {
            other.GetComponent<ObjetInteractible>().isMagneted = false;
        }
    }
}
