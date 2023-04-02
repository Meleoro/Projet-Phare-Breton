using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovements : MonoBehaviour
{
    [Header("Références")]
    [HideInInspector] public Fondu scriptFondu;
    private Camera _camera;
    private CameraRotationRef cameraRotationRefScript;

    [Header("CameraRoom")]
    private bool isStatic;
    [HideInInspector] public Vector3 minXZ;
    [HideInInspector] public Vector3 maxXZ;
    [HideInInspector] public Vector3 offset;

    [Header("DebutStatic")] 
    [SerializeField] private bool startMove;       // Si on veut que la camera bouge des le depart
    public Transform startMinXZ;
    public Transform startMaxXZ;

    [Header("Autres")]
    private Vector3 savePosition;     // Lorsque qu'on déplace un objet et qu'on change de camera avec, cette variable permet de retourner à la camera originelle
    private Quaternion saveRotation;
    private List<MeshRenderer> desactivatedObjects = new List<MeshRenderer>();


    private void Start()
    {
        _camera = GetComponent<Camera>();
        cameraRotationRefScript = GetComponentInChildren<CameraRotationRef>();
        scriptFondu = GetComponent<Fondu>();

        if (startMove)
        {
            isStatic = false;

            minXZ = startMinXZ.position;
            maxXZ = startMaxXZ.position;
        }

        else
        {
            isStatic = true;
        }
        
        ActualiseRotationCamRef();
    }


    private void Update()
    {
        if (!isStatic)
        {
            Vector3 charaPos = ReferenceManager.Instance.characterReference.transform.position;

            MoveCamera(charaPos);
        }
    }



    // PERMET DE DEPLACER LA CAMERA TOUT EN NE SORTANT DE CERTAINES LIMITES EN X ET EN Z
    public void MoveCamera(Vector3 wantedPos)
    {
        Vector3 newPos = new Vector3(0, 0, 0);

        // On determine la position en X
        if(wantedPos.x < minXZ.x)
        {
            newPos.x = minXZ.x;
        }
        else if(wantedPos.x > maxXZ.x)
        {
            newPos.x = maxXZ.x;
        }
        else
        {
            newPos.x = ReferenceManager.Instance.characterReference.transform.position.x;
        }

        // On determine la position en Z
        if (wantedPos.z < minXZ.z)
        {
            newPos.z = minXZ.z;
        }
        else if (wantedPos.z > maxXZ.z)
        {
            newPos.z = maxXZ.z;
        }
        else
        {
            newPos.z = ReferenceManager.Instance.characterReference.transform.position.z;
        }

        // Application des changements
        transform.position = new Vector3(newPos.x + offset.x, transform.position.y, newPos.z + offset.z);
    }
    
    

    // PERMET DE REORIENTER L'OBJET QUI NOUS SERT DE REFERENCE POUR L'ORIENTATION DES CONTROLES
    public void ActualiseRotationCamRef()
    {
        cameraRotationRefScript.currentRotation = new Vector3(0, transform.eulerAngles.y, 0);
    }


    // QUAND ON ENTRE DANS UNE PIECE
    public void EnterRoom(Vector3 newMinXZ, Vector3 newMaxXZ)
    {
        offset = transform.position - ReferenceManager.Instance.characterReference.transform.position;
        isStatic = false;

        minXZ = newMinXZ;
        maxXZ = newMaxXZ;
    }

    // QUAND ON QUITTE UNE PIECE
    public void ExitRoom()
    {
        isStatic = true;
    }


    // QUAND ON COMMENCE A CONTROLER UN OBJET
    public void SaveCamPos()
    {
        savePosition = transform.position;
        saveRotation = transform.rotation;
    }

    // QUAND ON ARRETE DE CONTROLER UN OBJET
    public void LoadCamPos()
    {
        transform.position = savePosition;
        transform.rotation = saveRotation;
    }


    // REND INVISIBLES LES OBJETS EN PARAMETRE
    public void ActualiseDesactivatedObjects(List<MeshRenderer> objects)
    {
        for (int k = 0; k < desactivatedObjects.Count; k++)
        {
            desactivatedObjects[k].gameObject.SetActive(true);
        }
        
        desactivatedObjects = objects;
        
        for (int k = 0; k < desactivatedObjects.Count; k++)
        {
            desactivatedObjects[k].gameObject.SetActive(false);
        }
    }
}
