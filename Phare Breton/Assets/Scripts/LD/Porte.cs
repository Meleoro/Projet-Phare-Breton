using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [Header("Changement Scene")] 
    [SerializeField] private bool changeScene;
    [SerializeField] private string sceneName;
    
    public void EnterDoor(GameObject movedObject, int doorNumber)
    {
        if (changeScene)
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            if (doorNumber == 1)
            {
                ReferenceManager.Instance.cameraReference.ActualiseDesactivatedObjects(door1.desactivatedObjects);
            }

            else
            {
                ReferenceManager.Instance.cameraReference.ActualiseDesactivatedObjects(door2.desactivatedObjects);
            }

            
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
                            CableThroughDoor(currentObject.cable.gameObject, movedObject, door1.gameObject, door2.gameObject);
                        
                        else
                            DestroyCableThroughDoor(door1, movedObject);
                    }

                    else
                    {
                        if(!door2.hasCableThrough) 
                            CableThroughDoor(currentObject.cable.gameObject, movedObject, door2.gameObject, door1.gameObject);
                        
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
        }
    }

    
    public void GoInside(int doorNumber, GameObject movedObject)
    {
        StartCoroutine(ReferenceManager.Instance.cameraReference.scriptFondu.EnterRoom(minXZ.position, maxXZ.position));
        
        if(doorNumber == 1)
            StartCoroutine(ReferenceManager.Instance.cameraReference.scriptFondu.Transition(charaPos2.position, cameraPos2, movedObject, this, doorNumber));
        
        else
            StartCoroutine(ReferenceManager.Instance.cameraReference.scriptFondu.Transition(charaPos1.position, cameraPos1, movedObject, this, doorNumber));
    }

    public void GoOutside(int doorNumber, GameObject movedObject)
    {
        StartCoroutine(ReferenceManager.Instance.cameraReference.scriptFondu.ExitRoom());
        
        if(doorNumber == 1)
            StartCoroutine(ReferenceManager.Instance.cameraReference.scriptFondu.Transition(charaPos2.position, cameraPos2, movedObject, this, doorNumber));
            
        else
            StartCoroutine(ReferenceManager.Instance.cameraReference.scriptFondu.Transition(charaPos1.position, cameraPos1, movedObject, this, doorNumber));
    }


    public void CableThroughDoor(GameObject currentCable, GameObject currentObject, GameObject endOldCable, GameObject startNewCable)
    {
        // Placement de l'ancien cable
        CableCreator currentScript = currentCable.GetComponent<CableCreator>();

        if (currentObject.CompareTag("Interactible"))
        {
            if(currentObject.GetComponent<ObjetInteractible>().isStart)
                currentScript.ChangeFirstNode(endOldCable, endOldCable.GetComponent<Rigidbody>(), null);
        
            else 
                currentScript.ChangeLastNode(endOldCable, endOldCable.GetComponent<Rigidbody>(), null);
        }
        else
        {
            currentScript.ChangeLastNode(endOldCable, endOldCable.GetComponent<Rigidbody>(), null);
        }


        // Recuperation des infos pour le nouveau cable
        float lenghtNewCable = currentScript.maxLength - currentScript.currentLength;
        int nbrNodesNewCable = (int) (currentScript.nbrMaxNodes * (currentScript.currentLength / currentScript.maxLength));


        // CREATION DU SECOND CABLE
        GameObject newCable = Instantiate(ReferenceManager.Instance.characterReference.fluteScript.ropeObject, startNewCable.transform.position, Quaternion.identity);
        CableCreator newScriptCreator = newCable.GetComponent<CableCreator>();
        Cable newScriptCable = newCable.GetComponent<Cable>();

        newScriptCreator.maxLength = lenghtNewCable;
        newScriptCreator.nbrMaxNodes = nbrNodesNewCable;

        newScriptCable.InitialiseStartEnd(startNewCable, currentObject);

        newScriptCreator.CreateNodes(currentObject.GetComponentInChildren<SpringJoint>(), startNewCable.GetComponent<SpringJoint>(), currentObject.GetComponent<ObjetInteractible>(), null,
            currentObject.GetComponent<Rigidbody>(), startNewCable.GetComponent<Rigidbody>());


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
