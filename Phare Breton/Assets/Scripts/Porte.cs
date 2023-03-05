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
    [SerializeField] Transform minX;
    [SerializeField] Transform maxX;
    [SerializeField] Transform minZ;
    [SerializeField] Transform maxZ;


    public void EnterDoor1()
    {
        ReferenceManager.Instance.characterReference.transform.position = charaPos2.position;

        ReferenceManager.Instance.cameraReference.transform.position = cameraPos2.position;
        ReferenceManager.Instance.cameraReference.transform.rotation = cameraPos2.rotation;
    }

    public void EnterDoor2()
    {
        ReferenceManager.Instance.characterReference.transform.position = charaPos1.position;

        ReferenceManager.Instance.cameraReference.transform.position = cameraPos1.position;
        ReferenceManager.Instance.cameraReference.transform.rotation = cameraPos1.rotation;
    }

    public void GoInside()
    {
        ReferenceManager.Instance.cameraReference.GetComponent<CameraMovements>().EnterRoom(minX.position.x, maxX.position.x, minZ.position.z, maxZ.position.z);
    }

    public void GoOutside()
    {
        ReferenceManager.Instance.cameraReference.GetComponent<CameraMovements>().ExitRoom();
    }
}
