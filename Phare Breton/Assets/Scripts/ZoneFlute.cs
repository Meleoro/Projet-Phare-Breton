using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class ZoneFlute : MonoBehaviour
{
    private List<GameObject> selectedObjects = new List<GameObject>(); 
    
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactible"))
        {
            ObjetInteractible _object = other.GetComponent<ObjetInteractible>();
            
            selectedObjects.Add(other.gameObject);
            _object.Select();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactible"))
        {
            ObjetInteractible _object = other.GetComponent<ObjetInteractible>();
            
            selectedObjects.Remove(other.gameObject);
            _object.Deselect();
        }
    }
}
