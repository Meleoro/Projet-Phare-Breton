using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraR : MonoBehaviour
{
    private Camera _camera;
    
    [Header("CameraRoom")]
    private bool isStatic;
    [HideInInspector] public float minX;
    [HideInInspector] public float maxX;
    [HideInInspector] public float minZ;
    [HideInInspector] public float maxZ;
    private Vector3 offset;



    private void Start()
    {
        _camera = GetComponent<Camera>();

        isStatic = true;
    }


    private void Update()
    {
        if (!isStatic)
        {
            Vector3 charaPos = ReferenceManager.Instance.characterReference.transform.position;

            MoveCamera(charaPos + offset);
        }
    }



    // PERMET DE DEPLACER LA CAMERA TOUT EN NE SORTANT DE CERTAINES LIMITES EN X ET EN Z
    private void MoveCamera(Vector3 wantedPos)
    {
        Vector3 newPos = new Vector3(0, transform.position.y, 0);

        // On determine la position en X
        if(wantedPos.x < minX)
        {
            newPos.x = minX;
        }
        else if(wantedPos.x > maxX)
        {
            newPos.x = minX;
        }
        else
        {
            newPos.x = transform.position.x;
        }

        // On determine la position en Z
        if (wantedPos.z < minZ)
        {
            newPos.z = minZ;
        }
        else if (wantedPos.z > maxZ)
        {
            newPos.z = minZ;
        }
        else
        {
            newPos.z = transform.position.z;
        }

        // Application des changements
        transform.position = newPos;
    }


    // QUAND ON ENTRE DANS UNE PIECE
    public void EnterRoom(Vector3 newOffset)
    {
        offset = newOffset;
        isStatic = false;
    }

    // QUAND ON QUITTE UNE PIECE
    public void ExitRoom()
    {
        isStatic = true;
    }
}
