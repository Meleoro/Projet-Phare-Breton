using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFlute : MonoBehaviour
{
    [Header("References")] 
    public GameObject zoneFlute;
    private CharaManager manager;

    private void Start()
    {
        manager = GetComponent<CharaManager>();
    }

    public void FluteActive(Vector2 direction)
    {
        manager.noMovement = true;
        zoneFlute.SetActive(true);
        
        zoneFlute.transform.rotation = Quaternion.LookRotation(new Vector3(direction.y, 0, -direction.x), Vector3.up);
    }

    public void FluteUnactive()
    {
        manager.noMovement = false;
        zoneFlute.SetActive(false);
    }
}
