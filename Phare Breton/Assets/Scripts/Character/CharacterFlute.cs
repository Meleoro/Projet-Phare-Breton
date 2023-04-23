using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;
using Object = System.Object;

public class CharacterFlute : MonoBehaviour
{
    [Header("General")]
    public List<ObjetInteractible> selectedObjects = new List<ObjetInteractible>();
    private bool doOnce;
    private bool onZone;
    private bool onVisee;
    private bool switch1;
    private bool switch2;

    [Header("Rope")] 
    public GameObject ropeObject;
    [HideInInspector] public List<GameObject> cables = new List<GameObject>();
    private List<ObjetInteractible> ropedObject = new List<ObjetInteractible>();

    [Header("References")] 
    public GameObject zoneFlute;
    public GameObject modeVisée;
    public GameObject modeZone;
    public AutoAimScript autoAimScript;
    public GameObject cablePoint;
    public GameObject VFXFluteUsed;
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
                
                for (int i = selectedObjects.Count - 1; i >= 0; i--)
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

                for (int i = selectedObjects.Count - 1; i >= 0; i--)
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

        Vector2 newDirection = autoAimScript.ChooseDirection(direction);

        zoneFlute.transform.localRotation = Quaternion.LookRotation(new Vector3(newDirection.y, 0, -newDirection.x), Vector3.up);
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

    public void PlayVFX()
    {
        GameObject newVFX = Instantiate(VFXFluteUsed, transform.position + Vector3.down * 0.9f, Quaternion.identity);
        StartCoroutine(DestroyVFX(newVFX));
    }

    IEnumerator DestroyVFX(GameObject currentVFX)
    {
        yield return new WaitForSeconds(5);
        
        Destroy(currentVFX);
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
                currentCable.InitialiseStartEnd(selectedObjects[0].gameObject, gameObject);
                
                // On crée le câble physiquement
                currentCableCreator.CreateNodes(selectedObjects[0].GetComponentInChildren<SpringJoint>(), cablePoint.GetComponent<SpringJoint>(), 
                    selectedObjects[0], null, selectedObjects[0].GetComponent<Rigidbody>(), 
                    gameObject.GetComponent<Rigidbody>());
                
                // On récupère les informations sur le câble et les objets liés à lui
                cables.Add(newRope);
                ropedObject.Add(selectedObjects[0]);
                
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
                        currentCable.InitialiseStartEnd(selectedObjects[k].gameObject, selectedObjects[j].gameObject);

                        // On crée le câble physiquement
                        currentCableCreator.CreateNodes(selectedObjects[k].GetComponentInChildren<SpringJoint>(), selectedObjects[j].GetComponentInChildren<SpringJoint>(), selectedObjects[k], selectedObjects[j],
                            selectedObjects[k].GetComponent<Rigidbody>(), selectedObjects[j].GetComponent<Rigidbody>());
                        
                        // On informe les scripts des objets qu'ils sont liés
                        selectedObjects[k].linkedObject.Add(selectedObjects[j].gameObject);
                        selectedObjects[k].cable = currentCableCreator;
                        
                        selectedObjects[j].linkedObject.Add(selectedObjects[k].gameObject);
                        selectedObjects[j].cable = currentCableCreator;

                        // On vérifie si ces objets intéragissent entre eux
                        selectedObjects[k].VerifyLinkedObject();
                        selectedObjects[j].VerifyLinkedObject();
                    }
                }
            }
        }
    }


    // QUAND LE JOUEUR PLACE LE(S) CABLE(S) QU'IL TRANSPORTE SUR UN OBJET
    public void PlaceLien()
    {
        if (manager.nearObjects.Count == 1)
        {
            SpringJoint objectSpring = manager.nearObjects[0].GetComponentInChildren<SpringJoint>();
            
            for (int k = cables.Count - 1; k >= 0; k--)
            {
                CableCreator currentCableCreator = cables[k].GetComponent<CableCreator>();

                objectSpring.connectedBody = currentCableCreator.nodesRope[currentCableCreator.nodesRope.Count - 1]
                    .GetComponent<Rigidbody>();

                // On relie les objets physiquement 
                currentCableCreator.ChangeLastNode(manager.nearObjects[0], manager.nearObjects[0].GetComponent<Rigidbody>(), manager.nearObjects[0].GetComponentInChildren<SpringJoint>());
                cables.RemoveAt(k);

                // On informe les scripts de chaque objets qu'ils sont connectés 
                ropedObject[k].linkedObject.Add(manager.nearObjects[0]);
                manager.nearObjects[0].GetComponent<ObjetInteractible>().linkedObject.Add(ropedObject[k].gameObject);

                if (ropedObject[k].linkedObject[0] == ropedObject[k].gameObject)
                {
                    Destroy(currentCableCreator.gameObject);
                }

                // Vérification des objets liés
                ropedObject[k].VerifyLinkedObject();
            }

            manager.nearObjects[0].GetComponent<ObjetInteractible>().VerifyLinkedObject();

            SpringJoint charaSpring = cablePoint.GetComponent<SpringJoint>();
        
            charaSpring.spring = 0;
            charaSpring.connectedBody = null;

            manager.hasRope = false;
            
            ropedObject.Clear();
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
            Boite currentBoite;
            if(movedObject.TryGetComponent<Boite>(out currentBoite))
            {
                currentBoite.VFXDeplacement.Play(true);
            }

            movedObject.GetComponent<ObjetInteractible>().isMoved = true;
            
            manager.movedObjects.Add(movedObject.GetComponent<Rigidbody>());
            manager.scriptsMovedObjects.Add(movedObject.GetComponent<ObjetInteractible>());

            manager.scriptsMovedObjects[manager.scriptsMovedObjects.Count - 1].currentHauteur = manager.movementScript.hauteurObject + transform.position.y;
            
            manager.movedObjects[manager.movedObjects.Count - 1].isKinematic = false;
        }
        
        else
        {
            for (int k = 0; k < selectedObjects.Count; k++)
            {
                Boite currentBoite;
                if(selectedObjects[k].TryGetComponent<Boite>(out currentBoite))
                {
                    currentBoite.VFXDeplacement.Play(true);
                }
                
                selectedObjects[k].GetComponent<ObjetInteractible>().isMoved = true;
                
                manager.movedObjects.Add(selectedObjects[k].GetComponent<Rigidbody>());
                manager.scriptsMovedObjects.Add(selectedObjects[k].GetComponent<ObjetInteractible>());

                manager.scriptsMovedObjects[k].currentHauteur = manager.movementScript.hauteurObject + transform.position.y;
                
                VerifyLinkedObject(selectedObjects[k].GetComponent<ObjetInteractible>());
            }

            for (int i = 0; i < manager.movedObjects.Count; i++)
            {
                manager.movedObjects[i].isKinematic = false;
            }
        }

        ReferenceManager.Instance.cameraReference.SaveCamPos();
    }

    // QUAND LE JOUEUR ARRETE DE DEPLACER DES OBJETS
    public void StopMoveObject()
    {
        manager.isMovingObjects = false;
        manager.noMovement = false;

        for (int i = 0; i < manager.scriptsMovedObjects.Count; i++)
        {
            StartCoroutine(manager.scriptsMovedObjects[i].PutRigidbodyKinematic());
            
            Boite currentBoite;
            if(manager.scriptsMovedObjects[i].TryGetComponent<Boite>(out currentBoite))
            {
                currentBoite.VFXDeplacement.Stop(true);
            }
            
            manager.scriptsMovedObjects[i].isMoved = false;
        }

        manager.movedObjects.Clear();
        manager.scriptsMovedObjects.Clear();

        StartCoroutine(ReferenceManager.Instance.cameraReference.scriptFondu.TransitionMovedObject());
    }



    public void Stase()
    {
        for (int i = 0; i < selectedObjects.Count; i++)
        {
            if (!selectedObjects[i].isInStase)
            {
                selectedObjects[i].isInStase = true;
                selectedObjects[i].rb.isKinematic = true;
            }

            else
            {
                selectedObjects[i].isInStase = false;
                selectedObjects[i].rb.isKinematic = false;
            }
        }
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
