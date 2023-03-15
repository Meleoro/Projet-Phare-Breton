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
    [HideInInspector] public bool isMoved;
    [HideInInspector] public float currentHauteur;

    [Header("Références")]
    private Rigidbody rb;



    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


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

        if (isMagneted)
        {
            MagnetEffect();
        }
    }



    public void ActivateMagnet(Transform magnetPos)
    {
        magnetedPos = magnetPos;
        isMagneted = true;
    }

    public void MagnetEffect()
    {
        transform.rotation = magnetedPos.rotation;

        if (isLinked)
        {
            transform.position = Vector3.Lerp(transform.position, magnetedPos.position, Time.deltaTime * 1.5f);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, magnetedPos.position, Time.deltaTime * 1.5f);
        }
    }

    
    public void Select()
    {

    }

    public void Deselect()
    {

    }

    
    
    // VERIFIE QUEL TYPE D'OBJET EST CONNECTÉ
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
