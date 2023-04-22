using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAimScript : MonoBehaviour
{
    public List<ObjetInteractible> autoAimedObjects = new List<ObjetInteractible>();
    private List<Vector2> directionsAutoAim = new List<Vector2>();

    [SerializeField] private Transform zoneAncrage;



    // On calcule les direction entre notre perso et les objets possiblement autoaimables
    public void CalculateDirections()
    {
        directionsAutoAim = new List<Vector2>();
        
        for (int k = 0; k < autoAimedObjects.Count; k++)
        {
            if (Mathf.Abs(autoAimedObjects[k].transform.position.y - transform.transform.position.y) < 1f)
            {
                if (autoAimedObjects[k].VerifySelection())
                {
                    Vector3 newDirection = autoAimedObjects[k].transform.position - transform.position;
                    newDirection = zoneAncrage.InverseTransformDirection(newDirection);
               
                    Vector2 newDirectionVector2 = new Vector2(newDirection.x, newDirection.z).normalized;
                
                    directionsAutoAim.Add(newDirectionVector2);
                }
            } 
        }
    }


    // On choisit en fonction des son input actuel quelle direction choisir
    public Vector2 ChooseDirection(Vector2 inputDirection)
    {
        CalculateDirections();

        Vector2 directionChose = inputDirection;
        float minAngle = 30;
        
        for (int i = 0; i < directionsAutoAim.Count; i++)
        {
            float angleDifference = Vector3.Angle(inputDirection, directionsAutoAim[i]);

            if (angleDifference < minAngle)
            {
                minAngle = angleDifference;
                directionChose = directionsAutoAim[i];
            }
        }

        return directionChose;
    }
    
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactible") && !other.isTrigger)
        {
            autoAimedObjects.Add(other.GetComponent<ObjetInteractible>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactible") && !other.isTrigger)
        {
            autoAimedObjects.Remove(other.GetComponent<ObjetInteractible>());
        }
    }
}
