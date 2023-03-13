using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Porte : MonoBehaviour
{
    [SerializeField] Transform charaPos1;
    [SerializeField] Transform charaPos2;

    [SerializeField] Transform cameraPos1;
    [SerializeField] Transform cameraPos2;

    [Header("Limites Camera")]
    [SerializeField] Transform minXZ;
    [SerializeField] Transform maxXZ;


    public void EnterDoor1(GameObject movedObject)
    {
        if (movedObject.CompareTag("Interactible"))
        {
            movedObject.GetComponent<ObjetInteractible>().currentHauteur = charaPos2.position.y;
        }
        
        movedObject.transform.position = charaPos2.position;

        ReferenceManager.Instance.cameraReference.transform.position = cameraPos2.position;
        ReferenceManager.Instance.cameraReference.transform.rotation = cameraPos2.rotation;
    }

    public void EnterDoor2(GameObject movedObject)
    {
        if (movedObject.CompareTag("Interactible"))
        {
            movedObject.GetComponent<ObjetInteractible>().currentHauteur = charaPos1.position.y;

            if (movedObject.GetComponent<ObjetInteractible>().isLinked)
            {
                //CableThroughDoor();
            }
        }
        
        movedObject.transform.position = charaPos1.position;
        
        ReferenceManager.Instance.cameraReference.transform.position = cameraPos1.position;
        ReferenceManager.Instance.cameraReference.transform.rotation = cameraPos1.rotation;
    }

    public void GoInside()
    {
        ReferenceManager.Instance.cameraReference.GetComponent<CameraMovements>().EnterRoom(minXZ.position, maxXZ.position);
    }

    public void GoOutside()
    {
        ReferenceManager.Instance.cameraReference.GetComponent<CameraMovements>().ExitRoom();
    }


    public void CableThroughDoor(GameObject currentCable, GameObject currentObject,GameObject porte, GameObject startNewCable)
    {
        // Placement de l'ancien cable
        CableCreator currentScript = currentCable.GetComponent<CableCreator>();
        
        currentScript.ChangeLastNode(porte, null, null);

        // Recuperation des infos pour le nouveau cable
        float lenghtNewCable = currentScript.maxLength - currentScript.currentLength;
        int nbrNodesNewCable = (int) (currentScript.nbrMaxNodes * (currentScript.currentLength / currentScript.maxLength));

        // Creation du second cable
        GameObject newCable = Instantiate(
            ReferenceManager.Instance.characterReference.GetComponent<CharacterFlute>().ropeObject,
            startNewCable.transform.position, Quaternion.identity);

        CableCreator newScriptCreator = newCable.GetComponent<CableCreator>();
        Cable newScriptCable = newCable.GetComponent<Cable>();

        newScriptCreator.maxLength = lenghtNewCable;
        newScriptCreator.nbrMaxNodes = nbrNodesNewCable;
        
        newScriptCable.originAnchor = gameObject;    // A modifier
        newScriptCable.endAnchor = currentObject;
        
        newScriptCreator.CreateNodes(null, currentObject.GetComponent<SpringJoint>());
    }
}
