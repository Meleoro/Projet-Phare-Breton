using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.Timeline;

public class CharacterFlute : MonoBehaviour
{
    [Header("General")]
    [HideInInspector] public List<GameObject> selectedObjects = new List<GameObject>();
    private bool doOnce;
    private bool onZone;
    private bool onVisee;

    [Header("Rope")] 
    public GameObject ropeObject;
    [HideInInspector] public List<GameObject> objectsAtRange = new List<GameObject>();
    [HideInInspector] public List<GameObject> cables = new List<GameObject>();
    private List<ObjetInteractible> ropedObject = new List<ObjetInteractible>();

    [Header("References")] 
    public GameObject zoneFlute;
    public GameObject modeVisée;
    public GameObject modeZone;
    public GameObject cablePoint;
    private CharaManager manager;

    private void Start()
    {
        manager = GetComponent<CharaManager>();
    }

    
    // QUAND ON MAINTIENT R2
    public void FluteActive(Vector2 direction)
    {
        // Choix du mode de visée
        if (direction == Vector2.zero)
        {
            onZone = true;
            
            if (onVisee)
            {
                onVisee = false;
                
                for (int i = 0; i < selectedObjects.Count; i++)
                {
                    selectedObjects[i].GetComponent<ObjetInteractible>().Deselect();
                    selectedObjects.RemoveAt(i);
                }
            }
            
            modeVisée.SetActive(false);
            modeZone.SetActive(true);
        }
        else 
        { 
            onVisee = true;
            
            if (onZone)
            {
                onZone = false;

                for (int i = 0; i < selectedObjects.Count; i++)
                {
                    selectedObjects[i].GetComponent<ObjetInteractible>().Deselect();
                    selectedObjects.RemoveAt(i);
                }
            }
            
            modeVisée.SetActive(true); 
            modeZone.SetActive(false);
        }
        
        // On immobilise le joueur et oriente la visée
        manager.noMovement = true;
        zoneFlute.SetActive(true);
        doOnce = false;

        zoneFlute.transform.localRotation = Quaternion.LookRotation(new Vector3(direction.y, 0, -direction.x), Vector3.up);
    }

    
    // QUAND ON RELACHE R2
    public void FluteUnactive()
    {
        manager.noMovement = false;
        zoneFlute.SetActive(false);

        if (!doOnce)
        {
            for (int i = selectedObjects.Count - 1; i >= 0; i--)
            {
                selectedObjects[i].GetComponent<ObjetInteractible>().Deselect();
                selectedObjects.RemoveAt(i);
            }

            doOnce = true;
        }
    }



    // QUAND LE JOUEUR CREE UN CABLE AVEC SA FLUTE
    public void CreateLien()
    {
        if (selectedObjects.Count > 0)
        {
            if (selectedObjects.Count == 1)
            {
                // Références
                GameObject newRope = Instantiate(ropeObject, transform.position, Quaternion.identity);
                Cable currentCable = newRope.GetComponent<Cable>();
                CableCreator currentCableCreator = newRope.GetComponent<CableCreator>();

                // On place le début et la fin du câble
                currentCable.originAnchor = selectedObjects[0];
                currentCable.endAnchor = gameObject;

                currentCable.originOffset =  currentCableCreator.ChooseSpotCable(gameObject, selectedObjects[0]) - selectedObjects[0].transform.position;
                currentCable.endOffset = currentCableCreator.ChooseSpotCable(selectedObjects[0], gameObject) - transform.position;

                currentCable.ActualiseNodes();
                
                // On crée le câble physiquement
                currentCableCreator.CreateNodes(selectedObjects[0].GetComponentInChildren<SpringJoint>(), cablePoint.GetComponent<SpringJoint>(), 
                    selectedObjects[0].GetComponent<ObjetInteractible>(), null, selectedObjects[0].GetComponent<Rigidbody>(), 
                    gameObject.GetComponent<Rigidbody>());
                
                // On récupère les informations sur le câble et les objets liés à lui
                cables.Add(newRope);
                ropedObject.Add(selectedObjects[0].GetComponent<ObjetInteractible>());
                
                manager.hasRope = true;
            }

            else
            {
                for (int k = selectedObjects.Count - 1; k > 0; k--)
                {
                    for (int j = k - 1; j >= 0; j--)
                    {
                        // Références
                        GameObject newRope = Instantiate(ropeObject, transform.position, Quaternion.identity);
                        Cable currentCable = newRope.GetComponent<Cable>();
                        CableCreator currentCableCreator = newRope.GetComponent<CableCreator>();

                        // On place le début et la fin du câble
                        currentCable.originAnchor = selectedObjects[k];
                        currentCable.endAnchor = selectedObjects[j];

                        currentCable.originOffset =  currentCableCreator.ChooseSpotCable(selectedObjects[j], selectedObjects[k]) - selectedObjects[k].transform.position;
                        currentCable.endOffset = currentCableCreator.ChooseSpotCable(selectedObjects[k], selectedObjects[j]) - selectedObjects[j].transform.position;

                        currentCable.ActualiseNodes();

                        // On crée le câble physiquement
                        currentCableCreator.CreateNodes(selectedObjects[k].GetComponentInChildren<SpringJoint>(), selectedObjects[j].GetComponentInChildren<SpringJoint>(), 
                            selectedObjects[k].GetComponent<ObjetInteractible>(), selectedObjects[j].GetComponent<ObjetInteractible>(),
                            selectedObjects[k].GetComponent<Rigidbody>(), selectedObjects[j].GetComponent<Rigidbody>());
                        
                        // On informe les scripts des objets qu'ils sont liés
                        selectedObjects[k].GetComponent<ObjetInteractible>().linkedObject.Add(selectedObjects[j]);
                        selectedObjects[k].GetComponent<ObjetInteractible>().cable = currentCableCreator;
                        
                        selectedObjects[j].GetComponent<ObjetInteractible>().linkedObject.Add(selectedObjects[k]);
                        selectedObjects[j].GetComponent<ObjetInteractible>().cable = currentCableCreator;
                    }
                }
            }
        }
    }


    // QUAND LE JOUEUR PLACE LE(S) CABLE(S) QU'IL TRANSPORTE SUR UN OBJET
    public void PlaceLien()
    {
        if (objectsAtRange.Count == 1)
        {
            SpringJoint objectSpring = objectsAtRange[0].GetComponentInChildren<SpringJoint>();
            
            for (int k = cables.Count - 1; k >= 0; k--)
            {
                CableCreator currentCableCreator = cables[k].GetComponent<CableCreator>();

                objectSpring.connectedBody = currentCableCreator.nodesRope[currentCableCreator.nodesRope.Count - 1]
                    .GetComponent<Rigidbody>();

                // On relie les objets physiquement 
                currentCableCreator.ChangeLastNode(objectsAtRange[0], objectsAtRange[0].GetComponent<Rigidbody>(), objectsAtRange[0].GetComponentInChildren<SpringJoint>());
                cables.RemoveAt(k);

                // On informe les scripts de chaque objets qu'ils sont connectés 
                ropedObject[k].linkedObject.Add(objectsAtRange[0]);
                objectsAtRange[0].GetComponent<ObjetInteractible>().linkedObject.Add(ropedObject[k].gameObject);
            }
            
            SpringJoint charaSpring = cablePoint.GetComponent<SpringJoint>();
        
            charaSpring.spring = 0;
            charaSpring.connectedBody = null;

            manager.hasRope = false;
        }
    }



    // QUAND LE JOUEUR COMMENCE A DEPLACER UN/DES OBJETS AVEC SA FLUTE
    public void MoveObject(bool recursiveCall, GameObject movedObject)
    {
        zoneFlute.SetActive(false);
    
        manager.isMovingObjects = true;
        manager.noMovement = true;
        manager.nearObjects.Clear();

        if (recursiveCall)
        {
            manager.movedObjects.Add(movedObject.GetComponent<Rigidbody>());
            manager.scriptsMovedObjects.Add(movedObject.GetComponent<ObjetInteractible>());

            manager.scriptsMovedObjects[manager.scriptsMovedObjects.Count - 1].currentHauteur = manager.movementScript.hauteurObject + transform.position.y;
        }
        
        else
        {
            for (int k = 0; k < selectedObjects.Count; k++)
            {
                manager.movedObjects.Add(selectedObjects[k].GetComponent<Rigidbody>());
                manager.scriptsMovedObjects.Add(selectedObjects[k].GetComponent<ObjetInteractible>());

                manager.scriptsMovedObjects[k].currentHauteur = manager.movementScript.hauteurObject + transform.position.y;
                
                VerifyLinkedObject(selectedObjects[k].GetComponent<ObjetInteractible>());
            }
        }

        ReferenceManager.Instance.cameraReference.GetComponent<CameraMovements>().SaveCamPos();
    }

    // QUAND LE JOUEUR ARRETE DE DEPLACER DES OBJETS
    public void StopMoveObject()
    {
        manager.isMovingObjects = false;
        manager.noMovement = false;

        manager.movedObjects.Clear();
        manager.scriptsMovedObjects.Clear();

        ReferenceManager.Instance.cameraReference.GetComponent<CameraMovements>().LoadCamPos();
    }


    public void VerifyLinkedObject(ObjetInteractible currentScript)
    {
        if (currentScript.isLinked)
        {
            for (int k = 0; k < currentScript.linkedObject.Count; k++)
            {
                MoveObject(true, currentScript.linkedObject[k]);
            }
        }
    }
}
