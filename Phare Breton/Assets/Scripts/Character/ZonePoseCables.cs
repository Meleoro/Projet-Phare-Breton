using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonePoseCables : MonoBehaviour
{
    public LayerMask ignoreLayer;

    private List<ObjetInteractible> objectsAtRange = new List<ObjetInteractible>();


    private void Update()
    {
        VerifySelection();
    }


    public void VerifySelection()
    {
        ReferenceManager.Instance.characterReference.nearObjects.Clear();
        ReferenceManager.Instance.characterReference.nearBoxes.Clear();
        ReferenceManager.Instance.characterReference.nearBoxesUp.Clear();
        ReferenceManager.Instance.characterReference.nearBoxesDown.Clear();
        ReferenceManager.Instance.characterReference.nearLadder = null;
        
        for (int i = 0; i < objectsAtRange.Count; i++)
        {
            // Echelle
            if (objectsAtRange[i].TryGetComponent(out Echelle currentEchelle))
            {
                if (currentEchelle.VerifyUse(transform))
                {
                    ReferenceManager.Instance.characterReference.nearObjects.Add(objectsAtRange[i].gameObject);
                    ReferenceManager.Instance.characterReference.nearLadder = currentEchelle;
                }
            }
            
            // Boite
            else if (objectsAtRange[i].TryGetComponent(out Boite currentBoite))
            {
                if (!Physics.Raycast(objectsAtRange[i].transform.position, Vector3.up, 1.5f, ignoreLayer))
                {
                    if (transform.position.y - 0.5f < objectsAtRange[i].transform.position.y)
                    {
                        ReferenceManager.Instance.characterReference.nearObjects.Add(objectsAtRange[i].gameObject);
                        ReferenceManager.Instance.characterReference.nearBoxesUp.Add(objectsAtRange[i].gameObject);
                        ReferenceManager.Instance.characterReference.nearBoxes.Add(objectsAtRange[i].gameObject);
                    }

                    else
                    {
                        ReferenceManager.Instance.characterReference.nearObjects.Add(objectsAtRange[i].gameObject);
                        ReferenceManager.Instance.characterReference.nearBoxesDown.Add(objectsAtRange[i].gameObject);
                        ReferenceManager.Instance.characterReference.nearBoxes.Add(objectsAtRange[i].gameObject);
                    }
                }
            }
        }
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactible") && !other.isTrigger)
        {
            ObjetInteractible _object = other.GetComponent<ObjetInteractible>();
            
            objectsAtRange.Add(_object);
            
            if (_object.TryGetComponent<Note>(out Note currentNote))
            {
                ReferenceManager.Instance.characterReference.nearObjects.Add(other.gameObject);
                ReferenceManager.Instance.characterReference.nearNoteObject = _object.gameObject;
                ReferenceManager.Instance.characterReference.nearNotePartitionNumber = currentNote.partitionNumber;
                ReferenceManager.Instance.characterReference.nearNoteNumber = currentNote.posInPartitionNumber;
            }

            /*if (VerifySelection(other.gameObject))
            {
                _object.Select();
                ReferenceManager.Instance.characterReference.nearObjects.Add(other.gameObject);
            }*/
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactible") && !other.isTrigger)
        {   
            ObjetInteractible _object = other.GetComponent<ObjetInteractible>();
            
            objectsAtRange.Remove(_object);
            
            if (_object.TryGetComponent<Note>(out Note currentNote))
            {
                ReferenceManager.Instance.characterReference.nearObjects.Remove(other.gameObject);
                ReferenceManager.Instance.characterReference.nearNoteObject = _object.gameObject;
                ReferenceManager.Instance.characterReference.nearNotePartitionNumber = 0;
                ReferenceManager.Instance.characterReference.nearNoteNumber = 0;
            }

            /*if (ReferenceManager.Instance.characterReference.nearObjects.Contains(other.gameObject))
            {
                _object.Deselect();
                ReferenceManager.Instance.characterReference.nearObjects.Remove(other.gameObject);
            
                if (_object.TryGetComponent<Note>(out Note currentNote))
                {
                    ReferenceManager.Instance.characterReference.nearObjects.Remove(other.gameObject);
                    ReferenceManager.Instance.characterReference.nearNoteObject = _object.gameObject;
                    ReferenceManager.Instance.characterReference.nearNotePartitionNumber = 0;
                    ReferenceManager.Instance.characterReference.nearNoteNumber = 0;
                }
            }*/
        }
    }
    
}
