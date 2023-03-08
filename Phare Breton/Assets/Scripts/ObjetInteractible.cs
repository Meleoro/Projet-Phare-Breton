using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjetInteractible : MonoBehaviour
{
    [Header("General")]
    public bool isLighted;
    public List<GameObject> linkedObject = new List<GameObject>();
    public enum InteractiblesType {
        carton,
        panneauElectrique,
        ampoule
    }
    public InteractiblesType interactibleType;

    [Header("Ampoule")]
    [SerializeField] private bool ampouleActive;
    [SerializeField] private Light lightComponent;


    private void Update()
    {
        VerifyLinkedObject();

        if (ampouleActive)
        {
            Ampoule();
        }
    }


    // VERIFIE QUEL TYPE D'OBJET EST CONNECTÉ
    private void VerifyLinkedObject()
    {
        if(linkedObject != null)
        {
            switch (interactibleType)
            {
                case InteractiblesType.carton:
                    break;


                case InteractiblesType.ampoule:

                    ampouleActive = false;

                    for (int k = 0; k < linkedObject.Count; k++)
                    {
                        if (linkedObject[k].GetComponent<ObjetInteractible>().interactibleType == InteractiblesType.panneauElectrique)
                        {
                            ampouleActive = true;
                        }
                    }

                    break;
            }
        }
    }


    // COMPORTEMENT DE L'OBJET SI IL EST UNE AMPOULE
    private void Ampoule()
    {
        lightComponent.enabled = true;
    }


    public void Select()
    {

    }

    public void Deselect()
    {

    }
}
