using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class ZoneFlute : MonoBehaviour
{
    [SerializeField] private CharacterFlute scriptFlute;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactible") && !other.isTrigger)
        {
            if (other.GetComponent<ObjetInteractible>().VerifySelection())
            {
                ObjetInteractible _object = other.GetComponent<ObjetInteractible>();

                scriptFlute.selectedObjects.Add(other.GetComponent<ObjetInteractible>());
                _object.Select();

                if (!other.TryGetComponent<Echelle>(out Echelle currentEchelle) && !other.TryGetComponent<Note>(out Note currentNote))
                {
                    scriptFlute.selectedObjectsCable.Add(other.GetComponent<ObjetInteractible>());
                }
                
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactible") && !other.isTrigger)
        {
            if (other.GetComponent<ObjetInteractible>().VerifySelection())
            {
                ObjetInteractible _object = other.GetComponent<ObjetInteractible>();

                scriptFlute.selectedObjects.Remove(other.GetComponent<ObjetInteractible>());
                _object.Deselect();
                
                if (!other.TryGetComponent<Echelle>(out Echelle currentEchelle) && !other.TryGetComponent<Note>(out Note currentNote))
                {
                    scriptFlute.selectedObjectsCable.Remove(other.GetComponent<ObjetInteractible>());
                }
            }
        }
    }
}
