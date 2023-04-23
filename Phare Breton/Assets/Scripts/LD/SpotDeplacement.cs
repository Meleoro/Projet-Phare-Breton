using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotDeplacement : MonoBehaviour
{
    [Header("Parametres")]
    public bool objectHasToBeRoped;

    public bool isUsed;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactible") && !other.isTrigger && !isUsed)
        {
            if (objectHasToBeRoped)
            {
                if(other.GetComponent<ObjetInteractible>().isLinked)
                {
                    other.GetComponent<ObjetInteractible>().ActivateMagnet(transform);
                    isUsed = true;
                }
            }
            else
            {
                other.GetComponent<ObjetInteractible>().ActivateMagnet(transform);
                isUsed = true;
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactible") && !other.isTrigger)
        {
            if (other.GetComponent<ObjetInteractible>().currentMagnet == gameObject)
            {
                isUsed = false;

                other.GetComponent<ObjetInteractible>().DesactivateMagnet(transform);
            }
        }
    }


    private void OnDrawGizmos()
    {
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.matrix = rotationMatrix;

        Gizmos.color = Color.blue;
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
    }
}
