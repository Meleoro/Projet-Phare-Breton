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
            
            if(_object.isClimbable)
                ReferenceManager.Instance.characterReference.nearObjects.Add(other.gameObject);
            
            if (_object.isNote)
            {
                ReferenceManager.Instance.characterReference.nearObjects.Add(other.gameObject);
                ReferenceManager.Instance.characterReference.nearNoteObject = _object.gameObject;
                ReferenceManager.Instance.characterReference.nearNotePartitionNumber = _object.partitionNumber;
                ReferenceManager.Instance.characterReference.nearNoteNumber = _object.posInPartitionNumber;
            }
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
            
            else if (_object.isNote)
            {
                ReferenceManager.Instance.characterReference.nearObjects.Remove(other.gameObject);
                ReferenceManager.Instance.characterReference.nearNoteObject = _object.gameObject;
                ReferenceManager.Instance.characterReference.nearNotePartitionNumber = 0;
                ReferenceManager.Instance.characterReference.nearNoteNumber = 0;
            }
        }
    }
}
