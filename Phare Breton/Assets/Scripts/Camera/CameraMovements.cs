using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CameraMovements : MonoBehaviour
{
    [Header("Références")]
    [HideInInspector] public Fondu scriptFondu;
    public Camera _camera;
    private CameraRotationRef cameraRotationRefScript;

    [Header("CameraRoom")]
    [HideInInspector] public bool isStatic;
    public Transform minXZ;
    [HideInInspector] public Transform maxXZ;
    [HideInInspector] public Vector3 offset;
    private Vector3 refMax;
    private Vector3 wantedPos;

    [Header("DebutStatic")] 
    [SerializeField] private bool startMove;       // Si on veut que la camera bouge des le depart
    public Transform startMinXZ;
    public Transform startMaxXZ;

    [Header("Autres")]
    private Vector3 savePosition;     // Lorsque qu'on déplace un objet et qu'on change de camera avec, cette variable permet de retourner à la camera originelle
    private Quaternion saveRotation;
    private List<TransparencyObject> desactivatedObjects = new List<TransparencyObject>();


    private void Start()
    {
        _camera = GetComponent<Camera>();
        cameraRotationRefScript = GetComponentInChildren<CameraRotationRef>();
        scriptFondu = GetComponent<Fondu>();

        if (startMove)
        {
            isStatic = false;

            minXZ = startMinXZ;
            maxXZ = startMaxXZ;
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

            Vector3 newPos = MoveCamera(charaPos);

            wantedPos = new Vector3(newPos.x + offset.x, transform.position.y, newPos.z + offset.z);
            transform.position = Vector3.Lerp(transform.position, wantedPos, Time.deltaTime * 3);
        }
    }



    // PERMET DE DEPLACER LA CAMERA TOUT EN NE SORTANT DE CERTAINES LIMITES EN X ET EN Z
    public Vector3 MoveCamera(Vector3 charaPos)
    {
        Vector3 newPos = new Vector3(0, 0, 0);
        charaPos = minXZ.InverseTransformPoint(ReferenceManager.Instance.characterReference.transform.position);
        
        
        if (charaPos.x < 0)
        {
            newPos.x = charaPos.x;
        }
        else if (charaPos.x > refMax.x)
        {
            newPos.x = charaPos.x - refMax.x;
        }

        
        if (charaPos.z < 0)
        {
            newPos.z = charaPos.z;
        }
        else if (charaPos.z > refMax.z)
        {
            newPos.z = charaPos.z - refMax.z;
        }
        
        return minXZ.TransformPoint(newPos);
    }


    public void InitialiseNewZone(Transform min, Transform max)
    {
        minXZ = min;
        refMax = min.InverseTransformPoint(max.position);
    }
    
    

    // PERMET DE REORIENTER L'OBJET QUI NOUS SERT DE REFERENCE POUR L'ORIENTATION DES CONTROLES
    public void ActualiseRotationCamRef()
    {
        cameraRotationRefScript.currentRotation = new Vector3(0, transform.eulerAngles.y, 0);
    }


    // QUAND ON ENTRE DANS UNE PIECE
    public void EnterRoom(bool staticCamera)
    {
        if (!staticCamera)
        {
            offset = transform.position - ReferenceManager.Instance.characterReference.transform.position;
            isStatic = false;
        }
        else
        {
            isStatic = true;
        }
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
    public void ActualiseDesactivatedObjects(List<TransparencyObject> objects, float distMin, float distMax)
    {
        for (int k = 0; k < desactivatedObjects.Count; k++)
        {
            desactivatedObjects[k].meshRenderer.material.SetFloat("Opacity", 1);
        }
        
        desactivatedObjects = objects;
        
        for (int k = 0; k < desactivatedObjects.Count; k++)
        {
            if (!desactivatedObjects[k].alphaManuelle)
            {
                float distObject = Vector3.Distance(transform.position, desactivatedObjects[k].meshRenderer.transform.position);
                distObject -= distMin;
                distObject /= distMax - distMin;
                
                desactivatedObjects[k].meshRenderer.material.SetFloat("_Opacity", distObject);
            }

            else
            {
                desactivatedObjects[k].meshRenderer.material.SetFloat("_Opacity", desactivatedObjects[k].alpha);
            }
        }
    }
}
