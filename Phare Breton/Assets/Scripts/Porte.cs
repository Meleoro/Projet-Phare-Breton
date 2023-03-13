using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Porte : MonoBehaviour
{
    [SerializeField] Transform charaPos1;
    [SerializeField] Transform charaPos2;

    [SerializeField] Transform cameraPos1;
    [SerializeField] Transform cameraPos2;

    [Header("Limites Camera")]
    [SerializeField] Transform minXZ;
    [SerializeField] Transform maxXZ;


    public void EnterDoor1(GameObject movedObject)
    {
        if (movedObject.CompareTag("Interactible"))
        {
            movedObject.GetComponent<ObjetInteractible>().currentHauteur = charaPos2.position.y;
        }
        
        movedObject.transform.position = charaPos2.position;

        ReferenceManager.Instance.cameraReference.transform.position = cameraPos2.position;
        ReferenceManager.Instance.cameraReference.transform.rotation = cameraPos2.rotation;
    }

    public void EnterDoor2(GameObject movedObject)
    {
        if (movedObject.CompareTag("Interactible"))
        {
            movedObject.GetComponent<ObjetInteractible>().currentHauteur = charaPos1.position.y;
        }
        
        movedObject.transform.position = charaPos1.position;

        ReferenceManager.Instance.cameraReference.transform.position = cameraPos1.position;
        ReferenceManager.Instance.cameraReference.transform.rotation = cameraPos1.rotation;
    }

    public void GoInside()
    {
        ReferenceManager.Instance.cameraReference.GetComponent<CameraMovements>().EnterRoom(minXZ.position, maxXZ.position);
    }

    public void GoOutside()
    {
        ReferenceManager.Instance.cameraReference.GetComponent<CameraMovements>().ExitRoom();
    }
}
