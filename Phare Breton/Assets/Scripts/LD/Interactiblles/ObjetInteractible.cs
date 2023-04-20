using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjetInteractible : MonoBehaviour
{
    [Header("General")]
    public bool isLighted;
    public bool isInDarkZone;
    public bool isClimbable;
    [HideInInspector] public bool isMagneted;
    [HideInInspector] public Transform magnetedPos;
    
    [Header("Link")]
    public bool isLinked;
    public List<GameObject> linkedObject = new List<GameObject>();
    [HideInInspector] public CableCreator cable;
    [HideInInspector] public bool isStart;

    [Header("MoveObject")]
    [HideInInspector] public bool isMoved;
    [HideInInspector] public float currentHauteur;

    [Header("Stase")]
    [HideInInspector] public bool isInStase;
                                                                                
    [Header("Références")]
    [HideInInspector] public Rigidbody rb;



    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(PutRigidbodyKinematic());

        isMagneted = false;
        isLighted = false;
    }


    private void Update()
    {
        if (isMagneted)
        {
            MagnetEffect();
        }
    }



    public void ActivateMagnet(Transform magnetPos)
    {
        magnetedPos = magnetPos;
        isMagneted = true;

        if (isLinked)
        {
            if (isStart)
            {
                cable.lockStart = true;
            }
            else 
            {
                cable.lockEnd = true;
            }
        }
    }
    
    public void DesactivateMagnet(Transform magnetPos)
    {
        magnetedPos = magnetPos;
        isMagneted = false;

        if (cable != null)
        {
            cable.lockStart = false;
            cable.lockEnd = false;
        }
    }
    

    public void MagnetEffect()
    {
        float magnetStrength = Vector3.Distance(magnetedPos.position, transform.position) * 1.3f;
        magnetStrength = (Time.deltaTime / magnetStrength) * 2.5f;
        
        transform.rotation = magnetedPos.rotation;
        transform.position = new Vector3(Mathf.Lerp(transform.position.x, magnetedPos.position.x, magnetStrength),
            Mathf.Lerp(transform.position.y, magnetedPos.position.y, magnetStrength), Mathf.Lerp(transform.position.z, magnetedPos.position.z, magnetStrength));


        float difference = magnetedPos.position.y - transform.position.y;
        
        if (difference > 0)
        {
            rb.AddForce(new Vector3(0, (-Physics.gravity.y + difference * 3) * Time.deltaTime, 0),
                ForceMode.VelocityChange);
        }

        else if (difference < -0.1f)
        {
            rb.AddForce(new Vector3(0, (difference * 3) * Time.deltaTime, 0),
                ForceMode.VelocityChange);
        }

        else 
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
    }



    public void Select()
    {

    }

    public void Deselect()
    {

    }

    public bool VerifySelection()
    {
        if (isInDarkZone)
        {
            if (isLighted)
                return true;

            else
                return false;
        }
        else
        {
            return true;
        }
    }
    


    public virtual void VerifyLinkedObject() { }
    

    public IEnumerator PutRigidbodyKinematic()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        
        yield return new WaitForSeconds(1);

        rb.isKinematic = true;
    }
}
