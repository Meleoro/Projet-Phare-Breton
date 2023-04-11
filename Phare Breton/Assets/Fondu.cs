using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Fondu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject canva;
    [SerializeField] private Image imageFondu;

    [Header("Parametres")]
    [SerializeField] private float dureeFondu;


    private void Start()
    {
        imageFondu.DOFade(0, 0);
        canva.SetActive(false);
    }


    public IEnumerator Transition(Vector3 objectNewPos, Transform cameraNewPos, GameObject movedObject, Porte doorScript, int doorNumber)
    {
        canva.SetActive(true);
        imageFondu.DOFade(1, dureeFondu);

        ReferenceManager.Instance.characterReference.noControl = true;

        yield return new WaitForSeconds(dureeFondu);
        
        ActualisePos(objectNewPos, cameraNewPos, movedObject);
        doorScript.EnterDoor(movedObject, doorNumber);

        ReferenceManager.Instance.cameraReference.ActualiseRotationCamRef();

        imageFondu.DOFade(0, dureeFondu);

        yield return new WaitForSeconds(dureeFondu);

        canva.SetActive(false);
        ReferenceManager.Instance.characterReference.noControl = false;
    }


    public IEnumerator EnterRoom(bool staticCamera)
    {
        yield return new WaitForSeconds(dureeFondu + 0.1f);

        ReferenceManager.Instance.cameraReference.EnterRoom(staticCamera);
    }



    public void ActualisePos(Vector3 objectNewPos, Transform cameraNewPos, GameObject gameObject)
    {
        gameObject.transform.position = objectNewPos;

        ReferenceManager.Instance.cameraReference.transform.position = cameraNewPos.position;
        ReferenceManager.Instance.cameraReference.transform.rotation = cameraNewPos.rotation;
    }
}
