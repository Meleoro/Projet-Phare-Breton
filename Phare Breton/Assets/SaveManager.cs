using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        
        else
            Destroy(gameObject);
    }


    public void TPZone1()
    {
        
    }

    public void TPZone21()
    {
        
    }

    public void TPZone22()
    {
        
    }

    public void TPZone31()
    {
        
    }

    public void TPZone32()
    {
        
    }
}
