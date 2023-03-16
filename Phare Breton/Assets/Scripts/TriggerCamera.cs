using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCamera : MonoBehaviour
{
    [SerializeField] private List<MeshRenderer> desactivatedObjects = new List<MeshRenderer>(); 
    
    [SerializeField] private BoxCollider _collider;
    [SerializeField] private Transform cameraPos;
    [SerializeField] private Color gizmosColor;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Interactible"))
        {
            // Points qui vont permettre de determiner de quel côté arrive le joueur
            Vector3 posAvant = transform.position + transform.forward;
            Vector3 posArriere = transform.position - transform.forward;

            
            if (Vector3.Distance(posArriere, other.transform.position) <
                Vector3.Distance(posAvant, other.transform.position))
            {
                ReferenceManager.Instance.cameraReference.transform.position = cameraPos.position;
                ReferenceManager.Instance.cameraReference.transform.rotation = cameraPos.rotation;
                
                ReferenceManager.Instance.cameraReference.ActualiseDesactivatedObjects(desactivatedObjects);
            }
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
