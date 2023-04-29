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

    [Header("Autres")]
    [HideInInspector] public bool isInTransition;
    [HideInInspector] public bool doorCrossed;


    private void Start()
    {
        imageFondu.DOFade(0, 0);
        canva.SetActive(false);
    }


    public IEnumerator Transition(Vector3 objectNewPos, Transform cameraNewPos, GameObject movedObject, Porte doorScript, int doorNumber, bool staticCamera)
    {
        SaveDoorCrossed();
        
        isInTransition = true;

        canva.SetActive(true);
        imageFondu.DOFade(1, dureeFondu);

        ReferenceManager.Instance.characterReference.noControl = true;

        yield return new WaitForSeconds(dureeFondu);
        
        ActualisePos(objectNewPos, cameraNewPos, movedObject);
        doorScript.EnterDoor(movedObject, doorNumber);

        ReferenceManager.Instance.cameraReference.ActualiseRotationCamRef();
        ReferenceManager.Instance.cameraReference.EnterRoom(staticCamera);

        imageFondu.DOFade(0, dureeFondu);

        yield return new WaitForSeconds(dureeFondu);

        canva.SetActive(false);
        ReferenceManager.Instance.characterReference.noControl = false;

        isInTransition = false;
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
