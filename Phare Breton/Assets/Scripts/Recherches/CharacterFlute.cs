using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

public class CharacterFlute : MonoBehaviour
{
    [HideInInspector] public List<GameObject> selectedObjects = new List<GameObject>();
    private bool doOnce;

    [Header("Rope")] public GameObject ropeObject;
    
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
            modeVisée.SetActive(false);
            modeZone.SetActive(true);
        }
        else
        {
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
            for (int i = 0; i < selectedObjects.Count; i++)
            {
                selectedObjects[i].GetComponent<ObjetInteractible>().Deselect();
                selectedObjects.RemoveAt(i);
            }

            doOnce = true;
        }
    }


    public void CreateLien()
    {
        if (selectedObjects.Count > 0)
        {
            for (int k = 0; k < selectedObjects.Count; k++)
            {
                GameObject newRope = Instantiate(ropeObject, transform.position, Quaternion.identity);

                Cable currentCable = newRope.GetComponent<Cable>();
                
                currentCable.originAnchor = selectedObjects[k].gameObject;
                currentCable.endAnchor = gameObject;
                
                
                CableCreator currentCableCreator = newRope.GetComponent<CableCreator>();

                currentCableCreator.origin.transform.position = selectedObjects[k].gameObject.transform.position;
                currentCableCreator.end.transform.position = gameObject.transform.position;
                
                currentCableCreator.CreateNodes();
            }

            manager.lien = false;
        }
    }
}
