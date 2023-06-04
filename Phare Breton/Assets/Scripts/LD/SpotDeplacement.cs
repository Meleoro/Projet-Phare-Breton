using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class SpotDeplacement : MonoBehaviour
{
    [Header("Parametres")]
    public bool objectHasToBeRoped;

    public bool isUsed;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactible") && !other.isTrigger && !isUsed)
        {
            ObjetInteractible currentObject = other.GetComponent<ObjetInteractible>();
            
            if (objectHasToBeRoped && !currentObject.cantMagnet)
            {
                if(currentObject.isLinked && !currentObject.isMagneted)
                {
                    currentObject.ActivateMagnet(transform);
                    isUsed = true;
                }
            }
            else if(!currentObject.isMagneted && !currentObject.cantMagnet)
            {
                currentObject.ActivateMagnet(transform);
                isUsed = true;
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactible") && !other.isTrigger)
        {
            ObjetInteractible currentObject = other.GetComponent<ObjetInteractible>();
            
            if (currentObject.currentMagnet == gameObject && isUsed)
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
