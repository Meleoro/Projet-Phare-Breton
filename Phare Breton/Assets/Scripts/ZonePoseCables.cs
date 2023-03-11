using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonePoseCables : MonoBehaviour
{
    public CharacterFlute scriptFlute;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactible"))
        {
            ObjetInteractible _object = other.GetComponent<ObjetInteractible>();
            
            scriptFlute.objectsAtRange.Add(other.gameObject);
            _object.Select();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactible"))
        {
            ObjetInteractible _object = other.GetComponent<ObjetInteractible>();
            
            scriptFlute.objectsAtRange.Remove(other.gameObject);
            _object.Deselect();
        }
    }
}
