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
    public Transform cameraPosRef;
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
    private Vector3 savePosition;
    private Quaternion saveRotation;
    private Transform saveMinXZ;
    private Transform saveMaxXZ;
    private Vector3 saveCameraRefPos;

    private List<TransparencyObject> desactivatedObjects = new List<TransparencyObject>();
    private float currentMinAlphaCamera;
    private float currentMaxAlphaCamera;
    private float currentMinAlphaChara;
    private float currentMaxAlphaChara;


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
            Vector3 newPos = MoveCamera();

            wantedPos = new Vector3(newPos.x, transform.position.y, newPos.z);
            transform.position = Vector3.Lerp(transform.position, wantedPos, Time.deltaTime * 3);
        }
        
        UpdateAlpha();
    }



    // PERMET DE DEPLACER LA CAMERA TOUT EN NE SORTANT DE CERTAINES LIMITES EN X ET EN Z
    public Vector3 MoveCamera()
    {
        Vector3 newPos = new Vector3(0, 0, 0);
        Vector3 charaPos = minXZ.InverseTransformPoint(ReferenceManager.Instance.characterReference.movedObjectPosition);
        
        
        /*if ((charaPos.x < 0 || charaPos.x > refMax.x) || (charaPos.z < 0 || charaPos.z > refMax.z))
        {
            newPos.x = charaPos.x;
            newPos.z = charaPos.z;
        }*/

        
        if (charaPos.x < 0)
        {
            newPos.x = charaPos.x;
        }
        else if (charaPos.x > refMax.x)
        {
            newPos.x = charaPos.x - refMax.x;
        }
        
        //newPos.y = charaPos.y;

         
        if (charaPos.z < 0)
        {
            newPos.z = charaPos.z;
        }
        else if (charaPos.z > refMax.z)
        {
            newPos.z = charaPos.z - refMax.z;
        }

        
        return cameraPosRef.TransformPoint(newPos);
    }


    public void InitialiseNewZone(Transform min, Transform max)
    {
        minXZ = min;
        maxXZ = max;

        cameraPosRef.rotation = minXZ.rotation;
        refMax = min.InverseTransformPoint(max.position);

        EnterRoom(false);
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

        saveMinXZ = minXZ;
        saveMaxXZ = maxXZ;

        saveCameraRefPos = cameraPosRef.position;
    }

    // QUAND ON ARRETE DE CONTROLER UN OBJET
    public void LoadCamPos()
    {
        transform.position = savePosition;
        transform.rotation = saveRotation;

        cameraPosRef.position = saveCameraRefPos;

        if (saveMinXZ != null)
            InitialiseNewZone(saveMinXZ, saveMaxXZ);

        else
            isStatic = true;
    }


    // REND INVISIBLES LES OBJETS EN PARAMETRE
    public void ActualiseDesactivatedObjects(List<TransparencyObject> objects, float distMinCamera, float distMaxCamera, float distMinChara, float distMaxChara)
    {
        for (int k = 0; k < desactivatedObjects.Count; k++)
        {
            for (int i = 0; i < desactivatedObjects[k].meshRenderers.Count; i++)
            {
                desactivatedObjects[k].meshRenderers[i].material.SetFloat("_alpha", 1);
            }
        }
        
        desactivatedObjects = objects;
        
        currentMinAlphaCamera = distMinCamera;
        currentMaxAlphaChara = distMaxCamera;

        currentMinAlphaChara = distMinChara;
        currentMaxAlphaChara = distMaxChara;
        
        UpdateAlpha();
    }

    public void UpdateAlpha()
    {
        for (int k = 0; k < desactivatedObjects.Count; k++)
        {
            for (int i = 0; i < desactivatedObjects[k].meshRenderers.Count; i++)
            {
                if (!desactivatedObjects[k].alphaManuelle)
                {
                    if (!desactivatedObjects[k].fromChara)
                    {
                        float distObject = Vector3.Distance(transform.position, desactivatedObjects[k].meshRenderers[i].transform.position);
                        distObject -= currentMinAlphaCamera;
                        distObject /= currentMaxAlphaCamera;

                        desactivatedObjects[k].meshRenderers[i].material.SetFloat("_alpha", distObject);
                    }
                    else
                    {
                        float distObject = Vector3.Distance(ReferenceManager.Instance.characterReference.transform.position, desactivatedObjects[k].meshRenderers[i].transform.position);
                        distObject -= currentMinAlphaChara;
                        distObject /= currentMaxAlphaChara;

                        desactivatedObjects[k].meshRenderers[i].material.SetFloat("_alpha", distObject);
                    }
                }

                else
                {
                    desactivatedObjects[k].meshRenderers[i].material.SetFloat("_alpha", desactivatedObjects[k].alpha);
                }
            }
        }
    }
}
