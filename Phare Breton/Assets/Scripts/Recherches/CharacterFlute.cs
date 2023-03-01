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

    public void FluteActive()
    {
        manager.noMovement = true;
        zoneFlute.SetActive(true);
    }

    public void FluteUnactive()
    {
        manager.noMovement = false;
        zoneFlute.SetActive(false);
    }
}
