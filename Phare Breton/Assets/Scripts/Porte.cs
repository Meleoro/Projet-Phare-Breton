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

    [SerializeField] GameObject door1;
    [SerializeField] GameObject door2;

    [Header("Limites Camera")]
    [SerializeField] Transform minXZ;
    [SerializeField] Transform maxXZ;


    public void EnterDoor1(GameObject movedObject)
    {
        if (movedObject.CompareTag("Interactible"))
        {
            movedObject.GetComponent<ObjetInteractible>().currentHauteur = charaPos2.position.y;

            if (movedObject.GetComponent<ObjetInteractible>().isLinked)
            {
                ObjetInteractible currentObject = movedObject.GetComponent<ObjetInteractible>();

                CableThroughDoor(currentObject.cable, movedObject, door1, door2);
            }
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
                ObjetInteractible currentObject = movedObject.GetComponent<ObjetInteractible>();

                CableThroughDoor(currentObject.cable, movedObject, door2, door1);
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


    public void CableThroughDoor(GameObject currentCable, GameObject currentObject, GameObject endOldCable, GameObject startNewCable)
    {
        // Placement de l'ancien cable
        CableCreator currentScript = currentCable.GetComponent<CableCreator>();
        
        currentScript.ChangeLastNode(endOldCable, null, null);


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


        newScriptCable.originAnchor = currentObject;
        newScriptCable.endAnchor = gameObject;

        newScriptCable.originOffset = currentObject.transform.position - newScriptCreator.ChooseSpotCable(gameObject, currentObject);
        newScriptCable.endOffset = transform.position - newScriptCreator.ChooseSpotCable(currentObject, gameObject);


        newScriptCreator.CreateNodes(null, currentObject.GetComponent<SpringJoint>(), null, null, currentObject.GetComponent<Rigidbody>());
    }
}
