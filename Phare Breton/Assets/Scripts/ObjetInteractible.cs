using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjetInteractible : MonoBehaviour
{
    [Header("General")]
    public bool isLighted;
    public bool isClimbable;
    [HideInInspector] public bool isMagneted;
    [HideInInspector] public Transform magnetedPos;
    public enum InteractiblesType {
        carton,
        panneauElectrique,
        ampoule
    }
    public InteractiblesType objectType;
    
    [Header("Link")]
    [HideInInspector] public bool isLinked;
    [HideInInspector] public List<GameObject> linkedObject = new List<GameObject>();
    [HideInInspector] public GameObject cable;
    [HideInInspector] public bool isStart;

    [Header("Ampoule")]
    [SerializeField] private bool ampouleActive;
    [SerializeField] private Light lightComponent;
    [SerializeField] private SphereCollider lightArea;

    [Header("MoveObject")] 
    [HideInInspector] public float currentHauteur;


    private void Update()
    {
        VerifyLinkedObject();

        if(objectType == InteractiblesType.ampoule)
        {
            if (ampouleActive)
            {
                ActivateAmpoule();
            }
        }
    }


    public void Magnet(Transform magnetPos)
    {
        magnetedPos = magnetPos;
        isMagneted = true;
    }
    
    public void Select()
    {

    }

    public void Deselect()
    {

    }

    
    
    // VERIFIE QUEL TYPE D'OBJET EST CONNECT�
    private void VerifyLinkedObject()
    {
        if (linkedObject != null)
        {
            switch (objectType)
            {
                case InteractiblesType.carton:
                    break;


                case InteractiblesType.ampoule:

                    ampouleActive = false;

                    for (int k = 0; k < linkedObject.Count; k++)
                    {
                        if (linkedObject[k].GetComponent<ObjetInteractible>().objectType == InteractiblesType.panneauElectrique)
                        {
                            ampouleActive = true;
                        }
                    }

                    break;
            }
        }
    }


    // COMPORTEMENT DE L'OBJET SI IL EST UNE AMPOULE
    private void ActivateAmpoule()
    {
        lightComponent.enabled = true;
        lightArea.enabled = true;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactible") && !other.isTrigger)
        {
            other.GetComponent<ObjetInteractible>().isLighted = true;
        }
    }

    /*private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactible") && !other.isTrigger)
        {
            other.GetComponent<ObjetInteractible>().isLighted = false;
        }
    }*/
}
