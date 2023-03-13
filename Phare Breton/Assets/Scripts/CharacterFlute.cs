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
    [HideInInspector] public bool hasRope;
    public List<GameObject> objectsAtRange = new List<GameObject>();
    private List<GameObject> cables = new List<GameObject>();
    private List<ObjetInteractible> ropedObject = new List<ObjetInteractible>();

    [Header("References")] 
    public GameObject zoneFlute;
    public GameObject modeVisée;
    public GameObject modeZone;
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

        zoneFlute.transform.localRotation = Quaternion.LookRotation(
            new Vector3(direction.y, 0,
                -direction.x), Vector3.up);
        
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
            for (int k = 0; k < selectedObjects.Count; k++)
            {
                // Références
                GameObject newRope = Instantiate(ropeObject, transform.position, Quaternion.identity);
                Cable currentCable = newRope.GetComponent<Cable>();
                CableCreator currentCableCreator = newRope.GetComponent<CableCreator>();

                // On place le début et la fin du câble
                currentCable.originAnchor = selectedObjects[k].gameObject;
                currentCable.endAnchor = gameObject;

                currentCableCreator.origin.transform.position = selectedObjects[k].gameObject.transform.position;
                currentCableCreator.end.transform.position = gameObject.transform.position;
                
                // On crée le câble physiquement
                currentCableCreator.CreateNodes(selectedObjects[k].GetComponent<SpringJoint>(), gameObject.GetComponent<SpringJoint>());
                
                // On récupère les informations sur le câble et les objets liés à lui
                cables.Add(newRope);
                ropedObject.Add(selectedObjects[k].GetComponent<ObjetInteractible>());
            }

            manager.lien = false;
            manager.hasRope = true;
        }
    }


    // QUAND LE JOUEUR PLACE LE(S) CABLE(S) QU'IL TRANSPORTE SUR UN OBJET
    public void PlaceLien()
    {
        if (objectsAtRange.Count == 1)
        {
            SpringJoint objectSpring = objectsAtRange[0].GetComponent<SpringJoint>();
            
            for (int k = cables.Count - 1; k >= 0; k--)
            {
                CableCreator currentCableCreator = cables[k].GetComponent<CableCreator>();

                objectSpring.connectedBody = currentCableCreator.nodesRope[currentCableCreator.nodesRope.Count - 2]
                    .GetComponent<Rigidbody>();

                currentCableCreator.springEnd = objectSpring;
                currentCableCreator.rbEnd = objectsAtRange[0].GetComponent<Rigidbody>();
                
                // On relie les objets physiquement 
                currentCableCreator.ChangePosNode(objectsAtRange[0]);
                cables.RemoveAt(k);

                // On informe les scripts de chaque objets qu'ils sont connectés 
                ropedObject[k].linkedObject.Add(objectsAtRange[0]);
                objectsAtRange[0].GetComponent<ObjetInteractible>().linkedObject.Add(ropedObject[k].gameObject);
            }
        }

        SpringJoint charaSpring = GetComponent<SpringJoint>();
        
        charaSpring.spring = 0;
        charaSpring.connectedBody = null;
        
        manager.hasRope = false;
    }
    
    
    
    // QUAND LE JOUEUR COMMENCE A DEPLACER UN/DES OBJETS AVEC SA FLUTE
    public void MoveObject()
    {
        zoneFlute.SetActive(false);
    
        manager.isMovingObjects = true;
        manager.noMovement = true;
        manager.nearObjects.Clear();

        for (int k = 0; k < selectedObjects.Count; k++)
        {
            manager.movedObjects.Add(selectedObjects[k].GetComponent<Rigidbody>());
            manager.scriptsMovedObjects.Add(selectedObjects[k].GetComponent<ObjetInteractible>());

            manager.scriptsMovedObjects[k].currentHauteur = manager.movementScript.hauteurObject + transform.position.y;
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
}
