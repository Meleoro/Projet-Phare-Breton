using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCredits : MonoBehaviour
{
    public Credits credits;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger && other.CompareTag("Player"))
        {
            credits.stop = false;
        }
    }
}
