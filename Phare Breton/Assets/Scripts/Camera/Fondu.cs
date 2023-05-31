using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Vector3 = UnityEngine.Vector3;

public class Fondu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject canva;
    [SerializeField] private Image imageFondu;

    [Header("Parametres")]
    [SerializeField] private float dureeFondu;
    [SerializeField] private float strengthMovement = 1;
    [SerializeField] private float strengthRotation = 1;

    [Header("Autres")]
    [HideInInspector] public bool isInTransition;
    [HideInInspector] public bool doorCrossed;
    
    [HideInInspector] public GameObject currentActivatedLight;


    private void Start()
    {
        StartCoroutine(StartScene());
    }

    public IEnumerator StartScene()
    {
        canva.SetActive(true);
        imageFondu.DOFade(1, 0);

        yield return new WaitForSeconds(0.1f);
        
        imageFondu.DOFade(0, dureeFondu * 2);

        yield return new WaitForSeconds(dureeFondu * 2);
        
        canva.SetActive(false);
    }


    public IEnumerator Transition(Vector3 objectNewPos, Transform cameraNewPos, GameObject movedObject, Porte doorScript, int doorNumber, bool staticCamera, GameObject activatedL, GameObject desactivatedL)
    {
        SaveDoorCrossed();

        //GetComponent<CutoutObject>().ResetAlphas();
        
        isInTransition = true;

        canva.SetActive(true);
        imageFondu.DOFade(1, dureeFondu);

        MoveCameraTransition(dureeFondu, true);

        ReferenceManager.Instance.characterReference.noControl = true;

        yield return new WaitForSeconds(dureeFondu * 1.2f);
        
        ActualisePos(objectNewPos, cameraNewPos, movedObject);
        doorScript.EnterDoor(movedObject, doorNumber);

        if(activatedL != null)
            activatedL.SetActive(true);

        currentActivatedLight = activatedL;
        
        if(desactivatedL != null)
            desactivatedL.SetActive(false);

        ReferenceManager.Instance.cameraReference.ActualiseRotationCamRef();
        ReferenceManager.Instance.cameraReference.EnterRoom(staticCamera);

        imageFondu.DOFade(0, dureeFondu);
        
        MoveCameraTransition(dureeFondu, false);

        yield return new WaitForSeconds(dureeFondu);

        canva.SetActive(false);
        ReferenceManager.Instance.characterReference.noControl = false;

        isInTransition = false;
    }
    
    public void MoveCameraTransition(float duration, bool goIn)
    {
        Vector3 posModificator = transform.TransformDirection(new Vector3(0, 2f, -3f)) * strengthMovement;
        Vector3 rotModificator = new Vector3(4, 0, 0) * strengthRotation;
        
        Vector3 newPos;
        Vector3 newRot;
        
        if (goIn)
        {
            newPos = transform.position + posModificator;
            newRot = transform.rotation.eulerAngles + rotModificator;
        }
        else
        {
            newPos = transform.position - posModificator;
            newRot = transform.rotation.eulerAngles - rotModificator;
        }
        
        transform.DOMove(newPos, duration).SetEase(Ease.OutQuad);
        transform.DORotate(newRot, duration).SetEase(Ease.OutQuad);
    }



    public void SaveDoorCrossed()
    {
        if (!doorCrossed)
        {
            if (ReferenceManager.Instance.characterReference.isMovingObjects)
            {
                doorCrossed = true;
            }
        }
    }
    

    public IEnumerator TransitionMovedObject()
    {
        if (doorCrossed)
        {
            isInTransition = true;
            ReferenceManager.Instance.cameraReference.isStatic = true;

            canva.SetActive(true);
            imageFondu.DOFade(1, dureeFondu);

            ReferenceManager.Instance.characterReference.noControl = true;

            yield return new WaitForSeconds(dureeFondu);

            imageFondu.DOFade(0, dureeFondu);
            ReferenceManager.Instance.cameraReference.isStatic = false;

            ReferenceManager.Instance.cameraReference.LoadCamPos();
            ReferenceManager.Instance.cameraReference.ActualiseRotationCamRef();

            yield return new WaitForSeconds(dureeFondu);

            ReferenceManager.Instance.characterReference.noControl = false;

            canva.SetActive(false);
            ReferenceManager.Instance.characterReference.noControl = false;

            doorCrossed = false;
            isInTransition = false;
        }

        else
        {
            ReferenceManager.Instance.cameraReference.LoadCamPos();
            ReferenceManager.Instance.cameraReference.ActualiseRotationCamRef();
        }
    }



    public void ActualisePos(Vector3 objectNewPos, Transform cameraNewPos, GameObject gameObject)
    {
        if(gameObject != null)
            gameObject.transform.position = objectNewPos;

        ReferenceManager.Instance.cameraReference.transform.position = cameraNewPos.position;
        ReferenceManager.Instance.cameraReference.transform.rotation = cameraNewPos.rotation;

        ReferenceManager.Instance.cameraReference.cameraPosRef.position = cameraNewPos.position;
    }
}
