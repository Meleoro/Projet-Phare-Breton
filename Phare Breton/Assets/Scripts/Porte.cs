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

    [SerializeField] EntreePorte door1;
    [SerializeField] EntreePorte door2;

    [Header("Limites Camera")]
    [SerializeField] Transform minXZ;
    [SerializeField] Transform maxXZ;
    
    
    public void EnterDoor(GameObject movedObject, int doorNumber)
    {
        if (movedObject.CompareTag("Interactible"))
        {
            if(doorNumber == 1)
                movedObject.GetComponent<ObjetInteractible>().currentHauteur = charaPos2.position.y;
            
            else
                movedObject.GetComponent<ObjetInteractible>().currentHauteur = charaPos1.position.y;

            
            if (movedObject.GetComponent<ObjetInteractible>().isLinked)
            {
                ObjetInteractible currentObject = movedObject.GetComponent<ObjetInteractible>();

                if (doorNumber == 1)
                {
                    if(!door1.hasCableThrough)
                        CableThroughDoor(currentObject.cable, movedObject, door1.gameObject, door2.gameObject);
                    
                    else
                        DestroyCableThroughDoor(door1, movedObject);
                }

                else
                {
                    if(!door2.hasCableThrough) 
                        CableThroughDoor(currentObject.cable, movedObject, door2.gameObject, door1.gameObject);
                    
                    else
                        DestroyCableThroughDoor(door2, movedObject);
                }
            }
        }

        if (movedObject.CompareTag("Player"))
        {
            if (movedObject.GetComponent<CharaManager>().hasRope)
            {
                CharacterFlute currentObject = movedObject.GetComponent<CharacterFlute>();

                for (int k = currentObject.cables.Count - 1; k >= 0; k--)
                {
                    if (doorNumber == 1)
                    {
                        if(!door1.hasCableThrough)
                            CableThroughDoor(currentObject.cables[k], movedObject, door1.gameObject, door2.gameObject);
                        
                        else
                            DestroyCableThroughDoor(door1, movedObject);
                    }
                    
                    else
                    {
                        if(!door2.hasCableThrough)
                            CableThroughDoor(currentObject.cables[k], movedObject, door2.gameObject, door1.gameObject);
                        
                        else
                            DestroyCableThroughDoor(door2, movedObject);
                    }
                }
            }
        }
        
        
        if (doorNumber == 1)
        {
            movedObject.transform.position = charaPos2.position;
            
            ReferenceManager.Instance.cameraReference.transform.position = cameraPos2.position;
            ReferenceManager.Instance.cameraReference.transform.rotation = cameraPos2.rotation;
        }

        else
        {
            movedObject.transform.position = charaPos1.position;
            
            ReferenceManager.Instance.cameraReference.transform.position = cameraPos1.position;
            ReferenceManager.Instance.cameraReference.transform.rotation = cameraPos1.rotation;
        }
        
        ReferenceManager.Instance.cameraReference.GetComponent<CameraMovements>().ActualiseRotationCamRef();
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

        if (currentObject.CompareTag("Interactible"))
        {
            if(currentObject.GetComponent<ObjetInteractible>().isStart)
                currentScript.ChangeFirstNode(endOldCable, null, null);
        
            else 
                currentScript.ChangeLastNode(endOldCable, null, null);
        }
        else
        {
            currentScript.ChangeLastNode(endOldCable, null, null);
        }


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
        newScriptCable.endAnchor = startNewCable;

        newScriptCable.originOffset = newScriptCreator.ChooseSpotCable(startNewCable, currentObject) - currentObject.transform.position;
        newScriptCable.endOffset = newScriptCreator.ChooseSpotCable(currentObject, startNewCable) - startNewCable.transform.position;


        newScriptCreator.CreateNodes(null, currentObject.GetComponent<SpringJoint>(), null, null,null, 
            currentObject.GetComponent<Rigidbody>());
        
        
        // On assigne tous ces éléments à la porte
        EntreePorte currentDoor = startNewCable.GetComponent<EntreePorte>();

        currentDoor.hasCableThrough = true;
        currentDoor.cableOtherSide = currentScript;
        currentDoor.cableThisSide = newScriptCreator;
    }

    
    public void DestroyCableThroughDoor(EntreePorte doorCrossed, GameObject currentObject)
    {
        if (doorCrossed.hasCableThrough)
        {
            // On détruit la partie du câble qui sert à rien
            Destroy(doorCrossed.cableThisSide.gameObject);
            
            
            // On relie le câble de l'autre côté à l'objet
            if (currentObject.CompareTag("Interactible"))
            {
                if(currentObject.GetComponent<ObjetInteractible>().isStart)
                    doorCrossed.cableOtherSide.ChangeFirstNode(currentObject, null, null);
        
                else 
                    doorCrossed.cableOtherSide.ChangeLastNode(currentObject, null, null);
            }
            else
            {
                doorCrossed.cableOtherSide.ChangeLastNode(currentObject, null, null);
            }
            
            
        }
    }
}
