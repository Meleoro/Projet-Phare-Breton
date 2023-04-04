using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cable : MonoBehaviour
{
    [HideInInspector] public GameObject originAnchor;
    [HideInInspector] public GameObject endAnchor;

    [HideInInspector] public Vector3 originOffset;
    [HideInInspector] public Vector3 endOffset;

    public Transform originNode;
    public Transform endNode;

    
    
    // SETUP DES DIFFERENTES VARIABLES
    public void InitialiseStartEnd(GameObject startObject, GameObject endObject)
    {
        originAnchor = startObject;
        endAnchor = endObject;

        originOffset =  ChooseSpotCable(endObject, startObject) - startObject.transform.position;
        endOffset = ChooseSpotCable(startObject, endObject) - endObject.transform.position;
        
        ActualiseNodes();
    }
    
    
    // ON REPLACE LE DEBUT ET LA FIN DU CABLE
    public void ActualiseNodes()
    {
        originNode.position = originAnchor.transform.position + originAnchor.transform.TransformDirection(originOffset);
        endNode.position = endAnchor.transform.position + endAnchor.transform.TransformDirection(endOffset);
    }
    
    
    // LANCE UN RAYCAST POUR TROUVER LE MEILLEUR ENDROIT OU PLACER L'EXTREMITE DU CABLE
    public Vector3 ChooseSpotCable(GameObject startObject, GameObject aimedObject)
    {
        Vector3 newDirection = aimedObject.transform.position - startObject.transform.position;
        Vector3 startPos = aimedObject.transform.position - newDirection.normalized * 2;

        RaycastHit raycastHit;

        if (Physics.Raycast(startPos, newDirection.normalized, out raycastHit, 2))
        {
            return raycastHit.point;
        }
        else
        {
            return aimedObject.transform.position;
        }
    }
}
