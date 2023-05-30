using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;


public class CameraMovements : MonoBehaviour
{
    [Header("Références")]
    [HideInInspector] public Fondu scriptFondu;
    public Camera _camera;
    public Transform cameraPosRef;
    public Volume rythmeVolume;
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

    [Header("PlayMusic")] 
    [HideInInspector] public Transform posCameraRythme;
    [HideInInspector] public Transform posCameraRythme2;
    [HideInInspector] public float durationRythme;
    private bool moveCameraRythme;
    private float timerMoveRythme;
    private Vector3 savePosRythme;
    private Quaternion saveRotRythme;
    [HideInInspector] public bool goToSave;


    [Header("CinematiqueIntro")]
    public string sceneStartName;
    public Transform posStart;
    public float duration;

    
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
    private bool isShaking;


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

        if (SceneManager.GetActiveScene().name == sceneStartName)
        {
            StartCoroutine(IntroCinematique());
        }
    }


    private void Update()
    {
        if (!isStatic && !goToSave)
        {
            if (!moveCameraRythme)
            {
                Vector3 newPos = MoveCamera();

                wantedPos = new Vector3(newPos.x, transform.position.y, newPos.z);
                transform.position = Vector3.Lerp(transform.position, wantedPos, Time.deltaTime * 3);
            }

            else
            {
                MoveCameraRythme();
            }
        }

        else if (moveCameraRythme)
        {
            MoveCameraRythme();
        }

        if (goToSave)
        {
            transform.position = Vector3.Lerp(transform.position, savePosRythme, Time.deltaTime * 5.5f);
            transform.rotation = Quaternion.Lerp(transform.rotation, saveRotRythme, Time.deltaTime * 6);

            if (Vector3.Distance(transform.position, savePosRythme) < 0.01f)
            {
                goToSave = false;
            }
        }
        
        UpdateAlpha();
    }


    public IEnumerator IntroCinematique()
    {
        yield return new WaitForSeconds(0.02f);

        isStatic = true;
        ReferenceManager.Instance.characterReference.StartCinematique();

        Vector3 savePos = transform.position;
        Vector3 saveRot = transform.rotation.eulerAngles;

        transform.rotation = posStart.rotation;
        transform.DOMove(posStart.position, 0);

        yield return new WaitForSeconds(duration * 0.2f);

        transform.DORotate(saveRot, duration);
        transform.DOMove(savePos, duration);


        yield return new WaitForSeconds(duration * 0.8f);

        ReferenceManager.Instance.characterReference.EndCinematique();

        isStatic = false;

    }



    // PERMET DE DEPLACER LA CAMERA TOUT EN NE SORTANT DE CERTAINES LIMITES EN X ET EN Z
    public Vector3 MoveCamera()
    {
        Vector3 newPos = new Vector3(0, 0, 0);
        Vector3 charaPos = minXZ.InverseTransformPoint(ReferenceManager.Instance.characterReference.movedObjectPosition);

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

        
        return cameraPosRef.TransformPoint(newPos);
    }

    public void MoveCameraRythme()
    {
        if (!isShaking)
        {
            timerMoveRythme += Time.deltaTime;

            if (timerMoveRythme < 2)
            {
                transform.position = Vector3.Lerp(transform.position, posCameraRythme.position, timerMoveRythme * 0.1f);
                transform.rotation = Quaternion.Lerp(transform.rotation, posCameraRythme.rotation, timerMoveRythme * 0.1f);
            }

            else
            {
                float avancee = timerMoveRythme / durationRythme;
                float depart = 2 / durationRythme;
            
                Vector3 wantedPos = Vector3.Lerp(posCameraRythme.position, posCameraRythme2.position,  avancee - depart);
                Quaternion wanterRot = Quaternion.Lerp(posCameraRythme.rotation, posCameraRythme2.rotation, avancee - depart);

                transform.position = Vector3.Lerp(transform.position, wantedPos, Time.deltaTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, wanterRot, Time.deltaTime);
            }
        }
    }

    public void StartMoveCameraRythme()
    {
        timerMoveRythme = 0;
        moveCameraRythme = true;
        goToSave = false;

        savePosRythme = transform.position;
        saveRotRythme = transform.rotation;
    }
    
    public void RestartMoveCameraRythme()
    {
        timerMoveRythme = 0.5f;
        moveCameraRythme = true;
    }
    
    public void StopMoveCameraRythme()
    {
        timerMoveRythme = 0;
        moveCameraRythme = false;
        goToSave = true;
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

    public void DoCameraShake(float duration, float intensity)
    {
        isShaking = true;
        
        GetComponentInParent<Transform>().DOShakePosition(duration, intensity).OnComplete((() => isShaking = false));
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
