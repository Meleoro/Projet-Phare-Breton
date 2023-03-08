using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.Timeline;

public class CharacterFlute : MonoBehaviour
{
    [HideInInspector] public List<GameObject> selectedObjects = new List<GameObject>();
    private bool doOnce;
    private bool onZone;
    private bool onVisee;

    [Header("Rope")] 
    public GameObject ropeObject;
    [HideInInspector] public bool hasRope;
    [HideInInspector] public List<GameObject> objectsAtRange = new List<GameObject>();
    private List<GameObject> cables = new List<GameObject>();
    
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
                
                cables.Add(newRope);
            }

            manager.lien = false;
            manager.hasRope = true;
        }
    }

    public void PlaceLien()
    {
        if (objectsAtRange.Count == 1)
        {
            for (int k = cables.Count - 1; k >= 0; k--)
            {
                cables[k].GetComponent<CableCreator>().ChangePosNode(objectsAtRange[0]);
                cables.RemoveAt(k);
            }
        }

        manager.hasRope = false;
    }
    
    
    
}
