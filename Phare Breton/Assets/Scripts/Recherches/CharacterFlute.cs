using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

public class CharacterFlute : MonoBehaviour
{
    [HideInInspector] public List<GameObject> selectedObjects = new List<GameObject>();
    private bool doOnce;
    
    [Header("References")] 
    public GameObject zoneFlute;
    public GameObject modeVisée;
    public GameObject modeZone;
    private CharaManager manager;

    private void Start()
    {
        manager = GetComponent<CharaManager>();
    }

    public void FluteActive(Vector2 direction)
    {
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
        
        manager.noMovement = true;
        zoneFlute.SetActive(true);
        doOnce = false;

        zoneFlute.transform.localRotation = Quaternion.LookRotation(
            new Vector3(direction.y, 0,
                -direction.x), Vector3.up);
    }

    public void FluteUnactive()
    {
        manager.noMovement = false;
        zoneFlute.SetActive(false);

        if (!doOnce)
        {
            for (int i = 0; i < selectedObjects.Count; i++)
            {
                selectedObjects[i].GetComponent<ObjetInteractible>().Deselect();
                selectedObjects.Remove(selectedObjects[i]);
            }

            doOnce = true;
        }
    }
}
