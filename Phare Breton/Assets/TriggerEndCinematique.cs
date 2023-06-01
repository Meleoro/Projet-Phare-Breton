using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEndCinematique : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !other.isTrigger)
        {
            ReferenceManager.Instance.cameraReference.doEndCinematique = true;
        }
    }
}
