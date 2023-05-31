using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonePoseCables : MonoBehaviour
{
    public LayerMask ignoreLayer;

    public List<ObjetInteractible> objectsAtRange = new List<ObjetInteractible>();
    private List<GameObject> objetsRecuperablesAtRange = new List<GameObject>();

    public List<GameObject> cableObjects = new List<GameObject>();


    private void Update()
    {
        VerifySelection();

        if (ReferenceManager.Instance.characterReference.hasRope)
        {
            FindPlaceCable();
        }
    }


    
    public void FindPlaceCable()
    {
        ReferenceManager.Instance.characterReference.cableObject = null;

        int currentIndex = -1;
        float currentDistance = 100;
        
        for (int i = 0; i < cableObjects.Count; i++)
        {
            float distance = Vector3.Distance(ReferenceManager.Instance.characterReference.transform.position,
                cableObjects[i].transform.position);
            
            if (cableObjects[i].TryGetComponent(out Boite currentBoite))
            {
                if (distance < currentDistance)
                {
                    currentDistance = distance;
                    currentIndex = i;
                }
            }
            
            else if (cableObjects[i].TryGetComponent(out Ampoule currentAmpoule))
            {
                if (distance < currentDistance)
                {
                    currentDistance = distance;
                    currentIndex = i;
                }
            }
            
            else if (cableObjects[i].TryGetComponent(out PanneauElectrique currentPanneauElectrique))
            {
                if (distance < currentDistance)
                {
                    currentDistance = distance;
                    currentIndex = i;
                }
            }
        }

        if (currentIndex >= 0)
        {
            ReferenceManager.Instance.characterReference.cableObject = cableObjects[currentIndex];
        }
    }


    public void VerifySelection()
    {
        ReferenceManager.Instance.characterReference.nearObjects.Clear();
        ReferenceManager.Instance.characterReference.nearBoxes.Clear();
        ReferenceManager.Instance.characterReference.nearBoxesUp.Clear();
        ReferenceManager.Instance.characterReference.nearBoxesDown.Clear();
        ReferenceManager.Instance.characterReference.nearAmpoule.Clear();
        ReferenceManager.Instance.characterReference.nearGenerator.Clear();
        ReferenceManager.Instance.characterReference.cableObjects.Clear();
        ReferenceManager.Instance.characterReference.nearLadder = null;
        
        cableObjects.Clear();

        ReferenceManager.Instance.characterReference.nearObjetsRecuperables = objetsRecuperablesAtRange;
        
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
                cableObjects.Add(objectsAtRange[i].gameObject);
                
                if (RaycastOnBox(objectsAtRange[i].transform.position, 1.5f))
                {
                    if (ReferenceManager.Instance.characterReference.transform.position.y - 0.5f < objectsAtRange[i].transform.position.y + objectsAtRange[i].transform.localScale.y * 0.5f)
                    {
                        if (ReferenceManager.Instance.characterReference.transform.position.y - (objectsAtRange[i].transform.position.y -
                                                    objectsAtRange[i].transform.localScale.y * 0.5f) - 0.5f > 0)
                        {
                            ReferenceManager.Instance.characterReference.nearObjects.Add(objectsAtRange[i].gameObject);
                            ReferenceManager.Instance.characterReference.nearBoxesUp.Add(objectsAtRange[i].gameObject);
                            ReferenceManager.Instance.characterReference.nearBoxes.Add(objectsAtRange[i].gameObject);
                        }
                    }

                    else
                    {
                        Vector2 direction = ReferenceManager.Instance.characterReference.movementScript
                            .stockageDirection;

                        if (!Physics.Raycast(transform.position,
                                ReferenceManager.Instance.cameraRotationReference.transform.TransformDirection(
                                    new Vector3(direction.x, 0, direction.y)), 1.5f))
                        {
                            ReferenceManager.Instance.characterReference.nearObjects.Add(objectsAtRange[i].gameObject);
                            ReferenceManager.Instance.characterReference.nearBoxesDown.Add(objectsAtRange[i].gameObject);
                            ReferenceManager.Instance.characterReference.nearBoxes.Add(objectsAtRange[i].gameObject);
                        }
                        else
                        {
                            RaycastHit _raycastHit;

                            if (Physics.Raycast(transform.position,
                                    ReferenceManager.Instance.cameraRotationReference.transform.TransformDirection(
                                        new Vector3(direction.x, 0, direction.y)), out _raycastHit, 1.5f))
                            {
                                if (_raycastHit.transform.CompareTag("Door"))
                                {
                                    ReferenceManager.Instance.characterReference.nearObjects.Add(objectsAtRange[i].gameObject);
                                    ReferenceManager.Instance.characterReference.nearBoxesDown.Add(objectsAtRange[i].gameObject);
                                    ReferenceManager.Instance.characterReference.nearBoxes.Add(objectsAtRange[i].gameObject);
                                }
                            }
                        }
                    }
                }
            }
            
            // Ampoule
            else if (objectsAtRange[i].TryGetComponent(out Ampoule currentAmpoule))
            {
                cableObjects.Add(objectsAtRange[i].gameObject);
                
                ReferenceManager.Instance.characterReference.nearAmpoule.Add(objectsAtRange[i].gameObject);
                ReferenceManager.Instance.characterReference.nearObjects.Add(objectsAtRange[i].gameObject);
            }
            
            // Generateur
            else if (objectsAtRange[i].TryGetComponent(out PanneauElectrique currentPanneauElectrique))
            {
                cableObjects.Add(objectsAtRange[i].gameObject);
                
                ReferenceManager.Instance.characterReference.nearGenerator.Add(objectsAtRange[i].gameObject);
                ReferenceManager.Instance.characterReference.nearObjects.Add(objectsAtRange[i].gameObject);
            }
        }

        ReferenceManager.Instance.characterReference.cableObjects = cableObjects;
    }


    public bool RaycastOnBox(Vector3 origin, float distance)
    {
        RaycastHit raycastHit;

        if (distance < 0)
            return true;
        
        if (Physics.Raycast(origin, Vector3.up, out raycastHit, distance, ignoreLayer))
        {
            if (!raycastHit.collider.isTrigger)
            {
                return false;
            }
            else
            {
                return RaycastOnBox(raycastHit.point + Vector3.up * 0.01f, distance - raycastHit.distance);
            }
        }

        else
        {
            return true;
        }
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactible") && !other.isTrigger && !other.TryGetComponent<Note>(out Note currentNote))
        {
            ObjetInteractible _object = other.GetComponent<ObjetInteractible>();
            
            objectsAtRange.Add(_object);
        }
        
        else if (other.CompareTag("ObjetRecuperables"))
        {
            objetsRecuperablesAtRange.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactible") && !other.isTrigger && !other.TryGetComponent<Note>(out Note currentNote))
        {   
            ObjetInteractible _object = other.GetComponent<ObjetInteractible>();

            objectsAtRange.Remove(_object);
        }
        
        else if (other.CompareTag("ObjetRecuperables"))
        {
            objetsRecuperablesAtRange.Remove(other.gameObject);
        }
    }
    
}
