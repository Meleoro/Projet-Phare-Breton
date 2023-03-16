using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonePoseCables : MonoBehaviour
{
    public CharacterFlute scriptFlute;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactible") && !other.isTrigger)
        {
            ObjetInteractible _object = other.GetComponent<ObjetInteractible>();
            
            scriptFlute.objectsAtRange.Add(other.gameObject);
            _object.Select();
            
            if(other.GetComponent<ObjetInteractible>().isClimbable)
                ReferenceManager.Instance.characterReference.nearObjects.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactible") && !other.isTrigger)
        {
            ObjetInteractible _object = other.GetComponent<ObjetInteractible>();
            
            scriptFlute.objectsAtRange.Remove(other.gameObject);
            _object.Deselect();
            
            if(other.GetComponent<ObjetInteractible>().isClimbable)
                ReferenceManager.Instance.characterReference.nearObjects.Remove(other.gameObject);
        }
    }
}
