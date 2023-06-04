using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CharacterObjects : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Image fond;
    [SerializeField] private GameObject UI;
    private CharaManager mainScript;

    [Header("ControlObject")] 
    public float accelerationRotation;
    public float vitesseRotation;
    private GameObject controlledObject;
    private Vector3 posObject;
    private float multiplier;
    private Vector2 stockageDirection;
    private bool canMove;
    
    
    private void Start()
    {
        mainScript = GetComponent<CharaManager>();

        UI.SetActive(true);
        fond.DOFade(0, 0);
    }



    public void PickUpObject(GameObject currentObject)
    {
        mainScript.isPickingObjectUp = true;
        posObject = ReferenceManager.Instance.cameraReference.transform.position +
                    ReferenceManager.Instance.cameraReference.transform.forward * currentObject.GetComponent<ObjetRecuperable>().distanceObjet;

        controlledObject = currentObject;

        StartCoroutine(StartPickUp());
    }

    public IEnumerator StartPickUp()
    {
        fond.DOFade(0.7f, 0.8f);
        controlledObject.transform.position = posObject;

        controlledObject.transform.parent = ReferenceManager.Instance.cameraReference.parentTranform;

        controlledObject.transform.DOScale(Vector3.zero, 0);
        controlledObject.transform.DOScale(Vector3.one, 0.8f);
        
        yield return new WaitForSeconds(0.8f);
        
        canMove = true;
    }
    
    public IEnumerator EndPickUp()
    {
        canMove = false;

        controlledObject.transform.DOScale(Vector3.one * 1.1f, 0.1f).SetEase(Ease.OutCubic);
        
        yield return new WaitForSeconds(0.1f);
        
        fond.DOFade(0f, 1f);
        
        controlledObject.transform.DOScale(Vector3.zero, 0.6f).SetEase(Ease.InCubic);
     
        yield return new WaitForSeconds(0.9f);

        Destroy(controlledObject);
        mainScript.isPickingObjectUp = false;
    }
    


    public void ControlObject(Vector2 direction, bool quitInput)
    {
        if (canMove)
        {
            if (quitInput)
                StartCoroutine(EndPickUp());

            direction = ReferenceManager.Instance.cameraRotationReference.transform.InverseTransformDirection(direction);

            Vector2 finalDirection = Vector2.zero;
        
            if (direction.magnitude > 0.1f)
            {
                if (multiplier < 1)
                {
                    multiplier += Time.deltaTime * accelerationRotation;
                }

                stockageDirection = direction;
                finalDirection = direction * (multiplier * vitesseRotation);
            }

            else
            {
                if (multiplier > 0)
                {
                    multiplier -= Time.deltaTime * accelerationRotation;
                }
            
                finalDirection = stockageDirection * (multiplier * vitesseRotation);
            }

            Vector3 realFinalDirection = ReferenceManager.Instance.cameraRotationReference.transform.TransformDirection(new Vector2(finalDirection.y, finalDirection.x));
        
            controlledObject.transform.Rotate(realFinalDirection, Space.World);
        }

        else
        {
            controlledObject.transform.Rotate(new Vector3(0, 0, 0));
        }
    }
}
