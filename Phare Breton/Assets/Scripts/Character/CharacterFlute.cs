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
    public List<ObjetInteractible> selectedObjectsCable = new List<ObjetInteractible>();
    private bool doOnce;
    private bool onZone;
    private bool onVisee;
    private bool switch1;
    private bool switch2;

    [Header("Rope")] 
    public GameObject ropeObject;
    [HideInInspector] public List<GameObject> cables = new List<GameObject>();
    private List<ObjetInteractible> ropedObject = new List<ObjetInteractible>();
    public List<Rigidbody> staticObjects = new List<Rigidbody>();

    [Header("References")] 
    public GameObject zoneFlute;
    public GameObject modeVisée;
    public GameObject modeZone;
    public ParticleSystemRenderer viseeVFX;
    public ParticleSystemRenderer zoneVFX;
    public AutoAimScript autoAimScript;
    public GameObject cablePoint;
    public GameObject VFXFluteUsed;
    private CharaManager manager;


    private void Start()
    {
        manager = GetComponent<CharaManager>();
        
        onZone = true;
        onVisee = true;
    }

    
    // QUAND ON MAINTIENT R2
    public void FluteActive(Vector2 direction)
    {
        // Choix du mode de visée
        if (direction.magnitude < 0.2f)
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
                
                for (int i = selectedObjectsCable.Count - 1; i >= 0; i--)
                {
                    selectedObjectsCable[i].GetComponent<ObjetInteractible>().Deselect();
                    selectedObjectsCable.RemoveAt(i);
                }
                
                StopAllCoroutines();
                StartCoroutine(ApparitionVFX(true));
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
                
                for (int i = selectedObjectsCable.Count - 1; i >= 0; i--)
                {
                    selectedObjectsCable[i].GetComponent<ObjetInteractible>().Deselect();
                    selectedObjectsCable.RemoveAt(i);
                }
                
                StopAllCoroutines();
                StartCoroutine(ApparitionVFX(false));
            }
            
            modeVisée.SetActive(true); 
            modeZone.SetActive(false);
            
            Vector2 newDirection = autoAimScript.ChooseDirection(direction);
            
            if(direction.magnitude > 0.03f)
                zoneFlute.transform.localRotation = Quaternion.LookRotation(new Vector3(newDirection.y, 0, -newDirection.x), Vector3.up);
        }
        
        // On immobilise le joueur et oriente la visée
        manager.noMovement = true;
        zoneFlute.SetActive(true);
        doOnce = false;
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
            
            for (int i = selectedObjectsCable.Count - 1; i >= 0; i--)
            {
                selectedObjectsCable[i].GetComponent<ObjetInteractible>().Deselect();
                selectedObjectsCable.RemoveAt(i);
            }

            onZone = true;
            onVisee = true;

            doOnce = true;
        }
    }

    IEnumerator ApparitionVFX(bool bulleVFX)
    {
        float avancee = 1;
        float wantedYBulle = 0;
        
        if (bulleVFX)
        {
            avancee = 1.5f;
            wantedYBulle = transform.position.y + 4;
        }

        while (avancee > 0)
        {
            if (bulleVFX)
            {
                zoneVFX.material.SetFloat("_CurOffHeight", wantedYBulle - avancee * 5);
            }
            else
            {
                viseeVFX.material.SetFloat("_CurOffHeight", 1 - avancee);
            }

            avancee -= Time.fixedDeltaTime * 4;

            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }

    public void PlayVFX()
    {
        GameObject newVFX = Instantiate(VFXFluteUsed, transform.position + Vector3.down * 0.9f, Quaternion.identity);

        Destroy(newVFX, 5);
        //StartCoroutine(DestroyVFX(newVFX));
    }



    // QUAND LE JOUEUR CREE UN CABLE AVEC SA FLUTE
    public void CreateLien()
    {
        if (selectedObjectsCable.Count > 0)
        {
            if (selectedObjectsCable.Count == 1)
            {
                if (!VerifyIfLinked(selectedObjectsCable[0]))
                {
                    AudioManager.instance.PlaySoundOneShot(11, 0, 0, manager.playerAudioSource);

                    // Références
                    GameObject newRope = Instantiate(ropeObject, transform.position, Quaternion.identity);
                    Cable currentCable = newRope.GetComponent<Cable>();
                    CableCreator currentCableCreator = newRope.GetComponent<CableCreator>();

                    // On place le début et la fin du câble
                    currentCable.InitialiseStartEnd(selectedObjectsCable[0].gameObject, gameObject);
                    
                    // On crée le câble physiquement
                    currentCableCreator.CreateNodes(selectedObjectsCable[0].GetComponentInChildren<SpringJoint>(), cablePoint.GetComponentInChildren<SpringJoint>(),
                        selectedObjectsCable[0], null, selectedObjectsCable[0].GetComponentInChildren<Rigidbody>(),
                        gameObject.GetComponentInChildren<Rigidbody>()); 

                    // On récupère les informations sur le câble et les objets liés à lui
                    cables.Add(newRope);
                    ropedObject.Add(selectedObjectsCable[0]);

                    manager.hasRope = true;
                }
            }

            else
            {
                float compteur = selectedObjectsCable.Count;

                for (int k = 0; k < selectedObjectsCable.Count; k++)
                {
                    if (VerifyIfLinked(selectedObjectsCable[k]))
                    {
                        Debug.Log(selectedObjectsCable[k].name);
                        
                        compteur -= 1;
                    }
                }

                if(compteur > 1)
                {
                    for (int k = selectedObjectsCable.Count - 1; k > 0; k--)
                    {
                        AudioManager.instance.PlaySoundOneShot(11, 0, 0, manager.playerAudioSource);
                        AudioManager.instance.PlaySoundOneShot(16, 0, 0, manager.playerAudioSource);

                        for (int j = k - 1; j >= 0; j--)
                        {
                            // Références
                            GameObject newRope = Instantiate(ropeObject, transform.position, Quaternion.identity);
                            Cable currentCable = newRope.GetComponent<Cable>();
                            CableCreator currentCableCreator = newRope.GetComponent<CableCreator>();

                            // On place le début et la fin du câble
                            currentCable.InitialiseStartEnd(selectedObjectsCable[k].gameObject, selectedObjectsCable[j].gameObject);

                            // On crée le câble physiquement
                            currentCableCreator.CreateNodes(selectedObjectsCable[k].GetComponentInChildren<SpringJoint>(), selectedObjectsCable[j].GetComponentInChildren<SpringJoint>(), selectedObjectsCable[k], selectedObjectsCable[j],
                                selectedObjectsCable[k].GetComponent<Rigidbody>(), selectedObjectsCable[j].GetComponent<Rigidbody>());

                            // On informe les scripts des objets qu'ils sont liés
                            selectedObjectsCable[k].linkedObject.Add(selectedObjectsCable[j].gameObject);
                            selectedObjectsCable[k].cable = currentCableCreator;

                            selectedObjectsCable[j].linkedObject.Add(selectedObjectsCable[k].gameObject);
                            selectedObjectsCable[j].cable = currentCableCreator;

                            // On vérifie si ces objets intéragissent entre eux
                            selectedObjectsCable[k].VerifyLinkedObject();
                            selectedObjectsCable[j].VerifyLinkedObject();
                        }

                        /*if (!VerifyIfLinked(selectedObjectsCable[k]))
                        {
                            AudioManager.instance.PlaySoundOneShot(11, 0, 0, manager.playerAudioSource);
                            AudioManager.instance.PlaySoundOneShot(16, 0, 0, manager.playerAudioSource);

                            for (int j = k - 1; j >= 0; j--)
                            {
                                // Références
                                GameObject newRope = Instantiate(ropeObject, transform.position, Quaternion.identity);
                                Cable currentCable = newRope.GetComponent<Cable>();
                                CableCreator currentCableCreator = newRope.GetComponent<CableCreator>();

                                // On place le début et la fin du câble
                                currentCable.InitialiseStartEnd(selectedObjectsCable[k].gameObject, selectedObjectsCable[j].gameObject);

                                // On crée le câble physiquement
                                currentCableCreator.CreateNodes(selectedObjectsCable[k].GetComponentInChildren<SpringJoint>(), selectedObjectsCable[j].GetComponentInChildren<SpringJoint>(), selectedObjectsCable[k], selectedObjectsCable[j],
                                    selectedObjectsCable[k].GetComponent<Rigidbody>(), selectedObjectsCable[j].GetComponent<Rigidbody>());

                                // On informe les scripts des objets qu'ils sont liés
                                selectedObjectsCable[k].linkedObject.Add(selectedObjectsCable[j].gameObject);
                                selectedObjectsCable[k].cable = currentCableCreator;

                                selectedObjectsCable[j].linkedObject.Add(selectedObjectsCable[k].gameObject);
                                selectedObjectsCable[j].cable = currentCableCreator;

                                // On vérifie si ces objets intéragissent entre eux
                                selectedObjectsCable[k].VerifyLinkedObject();
                                selectedObjectsCable[j].VerifyLinkedObject();
                            }
                        }*/
                    }
                }
                
            }
        }
    }

    public bool VerifyIfLinked(ObjetInteractible currentObject)
    {
        if (!currentObject.isLinked)
        {
            return false;
        }

        if(selectedObjectsCable.Count == 1)
        {
            List<GameObject> removedObjets = new List<GameObject>();

            for (int i = 0; i < currentObject.linkedObject.Count; i++)
            {
                removedObjets.Add(currentObject.linkedObject[i]);
            }

            for(int i = 0; i < removedObjets.Count; i++)
            {
                bool destroyOwnCable = false;

                if (removedObjets[i].GetComponent<ObjetInteractible>().cable == null)
                {
                    destroyOwnCable = true;
                }
                else if (currentObject.cable == null)
                {
                    destroyOwnCable = false;
                }
                        
                else if(removedObjets[i].GetComponent<ObjetInteractible>().cable.gameObject != currentObject.cable.gameObject)
                {
                    destroyOwnCable = true;
                }
                else
                {
                    destroyOwnCable = false;
                }

                ObjetInteractible currentLinkedObject = removedObjets[i].GetComponent<ObjetInteractible>();

                CableCreator currentCable = currentLinkedObject.cable;

                if (destroyOwnCable)
                    currentCable = currentObject.cable;


                Destroy(currentCable.gameObject);
                
                
                currentLinkedObject.linkedObject.Remove(currentObject.gameObject);
                if (currentLinkedObject.linkedObject.Count == 0)
                {
                    currentLinkedObject.isLinked = false;
                    Destroy(currentLinkedObject.cable.gameObject);
                }

                currentLinkedObject.VerifyLinkedObject();

                currentObject.linkedObject.Remove(currentLinkedObject.gameObject);
                if (currentObject.linkedObject.Count == 0)
                {
                    currentObject.isLinked = false;
                    Destroy(currentObject.cable.gameObject);
                }

                currentObject.VerifyLinkedObject();
            }

            for (int i = 0; i < removedObjets.Count; i++)
            {
                
            }
            
            


            /*currentObject.linkedObject.Clear();
            currentObject.isLinked = false;

            currentObject.VerifyLinkedObject();*/

            return true;
        }



        int original = currentObject.linkedObject.Count;
        int compteur = currentObject.linkedObject.Count;

        for (int i = 0; i < selectedObjectsCable.Count; i++)
        {
            if (currentObject.linkedObject.Contains(selectedObjectsCable[i].gameObject))
            {
                int index = 0;
                bool destroyOwnCable = false;

                for(int k = 0; k < currentObject.linkedObject.Count; k++)
                {
                    if (currentObject.linkedObject[k] == selectedObjectsCable[i].gameObject)
                    {
                        if (currentObject.linkedObject[k].GetComponent<ObjetInteractible>().cable == null)
                        {
                            destroyOwnCable = true;
                        }
                        else if (currentObject.cable == null)
                        {
                            destroyOwnCable = false;
                        }
                        
                        else if(currentObject.linkedObject[k].GetComponent<ObjetInteractible>().cable.gameObject != currentObject.cable.gameObject)
                        {
                            destroyOwnCable = true;
                        }
                        else
                        {
                            destroyOwnCable = false;
                        }

                        index = k;
                    }
                }

                ObjetInteractible currentLinkedObject = currentObject.linkedObject[index].GetComponent<ObjetInteractible>();
                    
                    
                CableCreator currentCable = currentLinkedObject.cable;

                if (destroyOwnCable)
                    currentCable = currentObject.cable;

                Destroy(currentCable.gameObject);


                currentLinkedObject.linkedObject.Remove(currentObject.gameObject);
                if(currentLinkedObject.linkedObject.Count == 0)
                    currentLinkedObject.isLinked = false;

                currentLinkedObject.VerifyLinkedObject();

                compteur -= 1;
                
                currentObject.linkedObject.Remove(currentLinkedObject.gameObject);
                if(currentObject.linkedObject.Count == 0)
                    currentObject.isLinked = false;
                
                currentObject.VerifyLinkedObject();
            }
            
        }


        if(compteur == original)
        {
            return false;
        }

        else
        {
            return true;
        }

        /*if (currentObject.isLinked)
        {
            for (int i = 0; i < currentObject.linkedObject.Count; i++)
            {
                ObjetInteractible currentLinkedObject = currentObject.linkedObject[i].GetComponent<ObjetInteractible>();
                CableCreator currentCable = currentLinkedObject.cable;

                Destroy(currentCable.gameObject);


                currentLinkedObject.linkedObject.Clear();
                currentLinkedObject.isLinked = false;

                currentLinkedObject.VerifyLinkedObject();
            }

            //ropedObject.Add(selectedObjectsCable[0]);

            currentObject.linkedObject.Clear();
            currentObject.isLinked = false;

            currentObject.VerifyLinkedObject();

            return true;
        }
        else
        {
            return false;
        }*/
    }


    // QUAND LE JOUEUR PLACE LE(S) CABLE(S) QU'IL TRANSPORTE SUR UN OBJET
    public void PlaceLien()
    {
        SpringJoint objectSpring = manager.cableObject.GetComponentInChildren<SpringJoint>();

        AudioManager.instance.PlaySoundOneShot(12, 0, 0, manager.playerAudioSource);

        for (int k = cables.Count - 1; k >= 0; k--)
        {
            CableCreator currentCableCreator = cables[k].GetComponent<CableCreator>();

            objectSpring.connectedBody = currentCableCreator.nodesRope[currentCableCreator.nodesRope.Count - 1]
                .GetComponent<Rigidbody>();

            // On relie les objets physiquement 

            if(currentCableCreator.rbOrigin == manager.rb)
            { 
                currentCableCreator.ChangeFirstNode(manager.cableObject, manager.cableObject.GetComponent<Rigidbody>(), manager.cableObject.GetComponentInChildren<SpringJoint>());
            }
            else
            { 
                currentCableCreator.ChangeLastNode(manager.cableObject, manager.cableObject.GetComponent<Rigidbody>(), manager.cableObject.GetComponentInChildren<SpringJoint>());
            }

            cables.RemoveAt(k);

            // On informe les scripts de chaque objets qu'ils sont connectés 
            ropedObject[k].linkedObject.Add(manager.cableObject);
            manager.cableObject.GetComponent<ObjetInteractible>().linkedObject.Add(ropedObject[k].gameObject);

            if (ropedObject[k].linkedObject[0] == ropedObject[k].gameObject)
            {
                ropedObject[k].isLinked = false;
                ropedObject[k].linkedObject.Clear();
                
                Destroy(currentCableCreator.gameObject);
            }

            // Vérification des objets liés
            ropedObject[k].VerifyLinkedObject();
        }

        manager.cableObject.GetComponent<ObjetInteractible>().VerifyLinkedObject();

        SpringJoint charaSpring = cablePoint.GetComponent<SpringJoint>();
        
        charaSpring.spring = 0;
        charaSpring.connectedBody = null;

        manager.hasRope = false;
            
        ropedObject.Clear();
        
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
            if (!movedObject.GetComponent<ObjetInteractible>().cantBeMoved)
            {
                Boite currentBoite;
                if(movedObject.TryGetComponent<Boite>(out currentBoite))
                {
                    currentBoite.VFXDeplacement.Play(true);
                }

                else if(movedObject.TryGetComponent<Planche>(out Planche currentPlanche))
                {
                    currentPlanche.VFXDeplacement.Play(true);
                }

                movedObject.GetComponent<ObjetInteractible>().isMoved = true;
                
                manager.movedObjects.Add(movedObject.GetComponent<Rigidbody>());
                manager.scriptsMovedObjects.Add(movedObject.GetComponent<ObjetInteractible>());

                manager.scriptsMovedObjects[manager.scriptsMovedObjects.Count - 1].currentHauteur = movedObject.transform.position.y;
                
                manager.movedObjects[manager.movedObjects.Count - 1].isKinematic = false;
            }
        }
        
        else
        {
            for (int k = 0; k < selectedObjects.Count; k++)
            {
                if(selectedObjects[k].gameObject == manager.objectOn)
                {
                    manager.isMovingObjects = false;
                    manager.noMovement = false;

                    manager.movedObjects.Clear();
                    manager.scriptsMovedObjects.Clear();
                    
                    StartCoroutine(manager.SayNo());
                    break;
                }
            }
                
            for (int k = 0; k < selectedObjects.Count; k++)
            { 
                if (!selectedObjects[k].cantBeMoved)
                {
                    if(selectedObjects[k].gameObject != manager.objectOn)
                    {
                        AudioManager.instance.PlaySoundContinuous(10, 0, 0, manager.playerAudioSource);
                        //AudioManager.instance.PlaySoundContinuous(15, 0, 0, manager.playerAudioSource);

                        manager.movementScript.colliderChara.enabled = false;
                        manager.rb.isKinematic = true;
                        
                        Boite currentBoite;
                        if (selectedObjects[k].TryGetComponent<Boite>(out currentBoite))
                        {
                            currentBoite.VFXDeplacement.Play(true);
                        }

                        else if (selectedObjects[k].TryGetComponent<Planche>(out Planche currentPlanche))
                        {
                            currentPlanche.VFXDeplacement.Play(true);
                        }

                        selectedObjects[k].GetComponent<ObjetInteractible>().isMoved = true;

                        manager.movedObjects.Add(selectedObjects[k].GetComponent<Rigidbody>());
                        manager.scriptsMovedObjects.Add(selectedObjects[k].GetComponent<ObjetInteractible>());

                        manager.scriptsMovedObjects[manager.scriptsMovedObjects.Count - 1].currentHauteur = selectedObjects[k].transform.position.y;

                        VerifyLinkedObject(selectedObjects[k].GetComponent<ObjetInteractible>());
                    }

                    else
                    {
                        manager.isMovingObjects = false;
                        manager.noMovement = false;

                        manager.movedObjects.Clear();
                        manager.scriptsMovedObjects.Clear();

                        StartCoroutine(manager.SayNo());

                        break;
                    }
                }
            }

            if (manager.movedObjects.Count != 0)
            {
                for (int i = 0; i < manager.movedObjects.Count; i++)
                {
                    manager.movedObjects[i].isKinematic = false;
                }
            }
            
            else
            {
                manager.isMovingObjects = false;
                manager.noMovement = false;
            }
        }

        ReferenceManager.Instance.cameraReference.SaveCamPos();
    }

    // QUAND LE JOUEUR ARRETE DE DEPLACER DES OBJETS
    public void StopMoveObject()
    {
        manager.playerAudioSource.Stop();

        manager.isMovingObjects = false;
        manager.noMovement = false;
        
        manager.movementScript.colliderChara.enabled = true;
        manager.rb.isKinematic = false;

        for (int i = 0; i < manager.scriptsMovedObjects.Count; i++)
        {
            manager.scriptsMovedObjects[i].rb.isKinematic = false;
                
            //StartCoroutine(manager.scriptsMovedObjects[i].PutRigidbodyKinematic());
            
            Boite currentBoite;
            if(manager.scriptsMovedObjects[i].TryGetComponent<Boite>(out currentBoite))
            {
                currentBoite.VFXDeplacement.Stop(true);
            }

            else if (manager.scriptsMovedObjects[i].TryGetComponent<Planche>(out Planche currentPlanche))
            {
                currentPlanche.VFXDeplacement.Stop(true);
            }

            if (manager.scriptsMovedObjects[i].rb.velocity.magnitude < 0.15f)
                manager.scriptsMovedObjects[i].rb.velocity = Vector3.down * 0.2f;
            
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
            if (!selectedObjects[i].cantBeMoved)
            {
                if (!selectedObjects[i].isInStase)
                {
                    AudioManager.instance.PlaySoundOneShot(13, 0, 0, manager.playerAudioSource);
                    //AudioManager.instance.PlaySoundOneShot(17, 0, 0, manager.playerAudioSource);

                    selectedObjects[i].ActivateStase();
                    selectedObjects[i].rb.isKinematic = true;

                    Boite currentBoite;
                    if (selectedObjects[i].TryGetComponent<Boite>(out currentBoite))
                    {
                        currentBoite.VFXDeplacement.Stop(true);
                    }
                }

                else
                {
                    AudioManager.instance.PlaySoundOneShot(14, 0, 0, manager.playerAudioSource);

                    selectedObjects[i].DesactivateStase();
                    selectedObjects[i].rb.isKinematic = false;
                }
            }
        }

        if(manager.isMovingObjects)
            StopMoveObject();
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
