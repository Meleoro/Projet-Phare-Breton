using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Fondu : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private GameObject canva;
    [SerializeField] private Image imageFondu;

    [Header("Paramètres")]
    [SerializeField] private float dureeFondu;


    private void Start()
    {
        imageFondu.DOFade(0, 0);
        canva.SetActive(false);
    }


    public IEnumerator Transition(Vector3 objectNewPos, Transform cameraNewPos, GameObject movedObject)
    {
        canva.SetActive(true);
        imageFondu.DOFade(1, dureeFondu);

        ReferenceManager.Instance.characterReference.noControl = true;

        yield return new WaitForSeconds(dureeFondu);

        ActualisePos(objectNewPos, cameraNewPos, movedObject);

        ReferenceManager.Instance.cameraReference.ActualiseRotationCamRef();

        imageFondu.DOFade(0, dureeFondu);

        yield return new WaitForSeconds(dureeFondu);

        canva.SetActive(false);
        ReferenceManager.Instance.characterReference.noControl = false;
    }

    public IEnumerator EnterRoom(Vector3 newMinXZ, Vector3 newMaxXZ)
    {
        yield return new WaitForSeconds(dureeFondu);

        ReferenceManager.Instance.cameraReference.EnterRoom(newMinXZ, newMaxXZ);
    }

    public IEnumerator ExitRoom()
    {
        yield return new WaitForSeconds(dureeFondu);

        ReferenceManager.Instance.cameraReference.ExitRoom();
    }



    public void ActualisePos(Vector3 objectNewPos, Transform cameraNewPos, GameObject gameObject)
    {
        gameObject.transform.position = objectNewPos;

        ReferenceManager.Instance.cameraReference.transform.position = cameraNewPos.position;
        ReferenceManager.Instance.cameraReference.transform.rotation = cameraNewPos.rotation;
    }
}
