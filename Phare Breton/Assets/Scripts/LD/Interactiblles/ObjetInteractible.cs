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
    [HideInInspector] public GameObject currentMagnet;
    [HideInInspector] public Transform magnetedPos;
    private bool isMagnetedBlock;
    private Vector3 originalPos;
    [HideInInspector] public bool mustVerify;
    
    [Header("Link")]
    public bool isLinked;
    public List<GameObject> linkedObject = new List<GameObject>();
    [HideInInspector] public CableCreator cable;
    [HideInInspector] public bool isStart;
    [HideInInspector] public Vector3 resistanceCable;

    [Header("MoveObject")] 
    public bool hauteurFigee;
    public float hauteurFigeeValeur = 2;
    private float wantedHauteur;
    public float hauteurRespawn = 20;
    [HideInInspector] public bool isMoved;
    [HideInInspector] public float currentHauteur;
    private float newPos;

    [Header("Stase")]
    [SerializeField] private ParticleSystem staseVFX;
    [HideInInspector] public bool isInStase;
                                                                                
    [Header("Références")]
    [HideInInspector] public Rigidbody rb;



    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(PutRigidbodyKinematic());

        /*Boite currentBoite;
        if(TryGetComponent(out currentBoite))
        {
            DesactivateStase();
        }*/
        
        isMagneted = false;
        isLighted = false;

        originalPos = transform.position;
        
        VerifyLinkedObject();
        StopVFX();

        wantedHauteur = transform.position.y + hauteurFigeeValeur;
    }


    private void Update()
    {
        if (isMagneted)
        {
            MagnetEffect();
        }

        if (originalPos.y - hauteurRespawn > transform.position.y)
        {
            transform.position = originalPos;
        }

        if (isMoved && !isMagneted)
        {
            ActualiseHauteur();
        }
    }


    public void ActivateStase()
    {
        staseVFX.Play();
        isInStase = true;
    }


    public void DesactivateStase()
    {
        staseVFX.Stop();
        isInStase = false;
    }


    public void ActualiseHauteur()
    {
        if (!hauteurFigee)
        {
            /*Ray ray = new Ray(transform.position, Vector3.down);
            RaycastHit raycastHit;

            if (Physics.Raycast(ray, out raycastHit, ReferenceManager.Instance.characterReference.movementScript.hauteurObject + transform.localScale.y * 0.5f))
            {
                if(!raycastHit.collider.CompareTag("Player") && !raycastHit.collider.isTrigger)
                {
                    if(raycastHit.collider.gameObject != gameObject)
                        if(transform.position.y - ReferenceManager.Instance.characterReference.movementScript.hauteurObject + 0.1f < raycastHit.point.y)
                            currentHauteur += Time.deltaTime * 2;
                }
            
            }*/

            if (DoRaycast(transform.position,
                    ReferenceManager.Instance.characterReference.movementScript.hauteurObject + (transform.localScale.y * 0.5f)))
            {
                currentHauteur = newPos;
            }

            else if (!DoRaycast(transform.position,
                         ReferenceManager.Instance.characterReference.movementScript.hauteurObject +
                         transform.localScale.y * 0.5f + 0.2f))
            {
                currentHauteur -= Time.deltaTime * 2;
            }
        }

        else
        {
            if (currentHauteur < wantedHauteur)
                currentHauteur = wantedHauteur;
        }
    }
    
    public bool DoRaycast(Vector3 startPos, float lenght)
    {
        Ray ray = new Ray(startPos, Vector3.down);
        RaycastHit raycastHit;

        if(lenght <= 0)
            return false;
        
        if (Physics.Raycast(ray, out raycastHit, lenght))
        {
            Debug.DrawLine(startPos, raycastHit.point);
            
            if (raycastHit.collider.isTrigger || raycastHit.collider.CompareTag("Player") || raycastHit.collider.gameObject == gameObject)
                return DoRaycast(raycastHit.point + Vector3.down * 0.01f, lenght - raycastHit.distance);

            newPos = raycastHit.point.y + ReferenceManager.Instance.characterReference.movementScript.hauteurObject + (transform.localScale.y * 0.5f);
            return true;
        }
        
        return false;
    }



    public void ActivateMagnet(Transform magnetPos)
    {
        magnetedPos = magnetPos;
        isMagneted = true;

        currentMagnet = magnetedPos.gameObject;

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

        currentMagnet = null;

        if (cable != null)
        {
            cable.lockStart = false;
            cable.lockEnd = false;
        }
    }
    

    public void MagnetEffect()
    {
        if (!isMagnetedBlock)
        {
            float magnetStrength = Vector3.Distance(magnetedPos.position, transform.position) * 1.3f;
            magnetStrength = (Time.deltaTime / magnetStrength) * 2.5f;
        
            transform.rotation = magnetedPos.rotation;
            transform.position = new Vector3(Mathf.Lerp(transform.position.x, magnetedPos.position.x, magnetStrength), 
                Mathf.Lerp(transform.position.y, magnetedPos.position.y, magnetStrength), Mathf.Lerp(transform.position.z, magnetedPos.position.z, magnetStrength));

            float difference = magnetedPos.position.y - transform.position.y;
            rb.velocity = new Vector3(rb.velocity.x, difference, rb.velocity.z);
        }

        if (Vector3.Distance(transform.position, magnetedPos.position) < 0.03f && !isMoved)
        {
            isMagnetedBlock = true;
            rb.isKinematic = true;
        }

        else
        {
            isMagnetedBlock = false;
            rb.isKinematic = false;
        }
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

    public virtual void StopVFX() { }


    public IEnumerator PutRigidbodyKinematic()
    {
        if(isInStase)
            rb.isKinematic = true;

        if(!rb.isKinematic)
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        
        yield return new WaitForSeconds(3);

        if(!isMoved)
            rb.isKinematic = true;
    }
}
