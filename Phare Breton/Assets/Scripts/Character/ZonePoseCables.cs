using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonePoseCables : MonoBehaviour
{
    public CharacterFlute scriptFlute;

    
    
    public bool VerifySelection(GameObject currentObject)
    {
        if (!Physics.Raycast(currentObject.transform.position, Vector3.up, 1.5f))
        {
            if (Mathf.Abs(transform.position.y - currentObject.transform.position.y) < 1.5f)
            {
                return true;
            }

            return false;
        }
        
        return false;
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactible") && !other.isTrigger)
        {
            ObjetInteractible _object = other.GetComponent<ObjetInteractible>();

            if (VerifySelection(other.gameObject))
            {
                _object.Select();
                ReferenceManager.Instance.characterReference.nearObjects.Add(other.gameObject);
            }

            if (_object.TryGetComponent<Note>(out Note currentNote))
            {
                ReferenceManager.Instance.characterReference.nearObjects.Add(other.gameObject);
                ReferenceManager.Instance.characterReference.nearNoteObject = _object.gameObject;
                ReferenceManager.Instance.characterReference.nearNotePartitionNumber = currentNote.partitionNumber;
                ReferenceManager.Instance.characterReference.nearNoteNumber = currentNote.posInPartitionNumber;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactible") && !other.isTrigger)
        {   
            ObjetInteractible _object = other.GetComponent<ObjetInteractible>();
            
            _object.Deselect();
            ReferenceManager.Instance.characterReference.nearObjects.Remove(other.gameObject);
            
            if (_object.TryGetComponent<Note>(out Note currentNote))
            {
                ReferenceManager.Instance.characterReference.nearObjects.Remove(other.gameObject);
                ReferenceManager.Instance.characterReference.nearNoteObject = _object.gameObject;
                ReferenceManager.Instance.characterReference.nearNotePartitionNumber = 0;
                ReferenceManager.Instance.characterReference.nearNoteNumber = 0;
            }
        }
    }
    
}
