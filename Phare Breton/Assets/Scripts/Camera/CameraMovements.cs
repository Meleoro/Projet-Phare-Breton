using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;


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


    [Header("CinematiqueMiddle")]
    public bool doMiddleCinematique;
    public Transform posCameraMiddle;
    public Transform posCharaMiddle1;
    public Transform posCharaMiddle2;
    public float durationMiddle;


    [Header("CinematiqueFin")]
    public bool doEndCinematique;
    public Transform posCameraEnd;
    public Transform posCharaEnd1;
    public Transform posCharaEnd2;
    public float durationEnd;



    [Header("ShakeCamera")] 
    public float amplitudeShake;
    public float speedShake;
    private float timerShake;
    private Vector2 currentWantedPosShake;
    private Vector2 wantedPosShake;
    private Vector3 posModificateur;
    private float screenWidth;
    private float screenHeight;
    public Transform parentTranform;
    private Vector3 originalPos;


    [Header("Autres")]
    private Vector3 savePosition;
    private Quaternion saveRotation;
    private Transform saveMinXZ;
    private Transform saveMaxXZ;
    private GameObject lightToActivate;
    private GameObject lightToDesactivate;
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
        
        originalPos = parentTranform.position;
        
        screenWidth = _camera.pixelWidth;
        screenHeight = _camera.pixelHeight;

        StartCoroutine(ShakeCoroutine());

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
        else if (doMiddleCinematique) 
        {
            StartCoroutine(MiddleCinematique());
        }
    }


    private void Update()
    {
        screenWidth = _camera.pixelWidth;
        screenHeight = _camera.pixelHeight;

        if (!isShaking)
        {
            parentTranform.position =
                Vector3.Lerp(parentTranform.position, originalPos + posModificateur, Time.deltaTime);
        }

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
    
    private IEnumerator ShakeCoroutine()
    {
        timerShake += Time.deltaTime;

        if (timerShake > speedShake)
        {
            timerShake = 0;

            Vector2 save = currentWantedPosShake;

            while (Vector2.Distance(save, currentWantedPosShake) < amplitudeShake)
            {
                currentWantedPosShake = new Vector2(Random.Range(-amplitudeShake, amplitudeShake), 
                    Random.Range(-amplitudeShake, amplitudeShake));
            }
        }
        
        wantedPosShake = Vector2.Lerp(wantedPosShake, currentWantedPosShake, Time.deltaTime * 0.5f / speedShake);
        posModificateur = Vector3.Lerp(posModificateur, transform.TransformDirection(new Vector3(wantedPosShake.x, wantedPosShake.y, 0)), Time.deltaTime * 0.25f / speedShake);

        yield return new WaitForSeconds(Time.deltaTime);

        StartCoroutine(ShakeCoroutine());
    }


    public IEnumerator IntroCinematique()
    {
        yield return new WaitForSeconds(0.02f);

        bool staticStock = isStatic; 
        
        isStatic = true;
        ReferenceManager.Instance.characterReference.StartCinematique();

        Vector3 savePos = transform.position;
        Vector3 saveRot = transform.rotation.eulerAngles;

        transform.rotation = posStart.rotation;
        transform.DOMove(posStart.position, 0);

        yield return new WaitForSeconds(duration * 0.15f);

        transform.DORotate(saveRot, duration);
        transform.DOMove(savePos, duration).SetEase(Ease.InOutSine);


        yield return new WaitForSeconds(duration * 0.85f);

        ReferenceManager.Instance.characterReference.EndCinematique();

        isStatic = staticStock;
    }


    public IEnumerator MiddleCinematique()
    {
        yield return new WaitForSeconds(0.02f);

        bool staticStock = isStatic;

        ReferenceManager.Instance.characterReference.transform.position = posCharaMiddle1.position;
        StartCoroutine(ReferenceManager.Instance.characterReference.movementScript.ClimbLadder(posCharaMiddle2.position, posCharaMiddle1.position, true));

        isStatic = true;

        Vector3 savePos = transform.position;
        Vector3 saveRot = transform.rotation.eulerAngles;

        transform.rotation = posCameraMiddle.rotation;
        transform.DOMove(posCameraMiddle.position, 0);

        yield return new WaitForSeconds(durationMiddle * 0.15f);

        transform.DORotate(saveRot, durationMiddle);
        transform.DOMove(savePos, durationMiddle).SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(durationMiddle * 0.85f);

        ReferenceManager.Instance.characterReference.EndCinematique();

        isStatic = staticStock;
    }


    public IEnumerator EndCinematique()
    {
        yield return new WaitForSeconds(0.02f);

        ReferenceManager.Instance.characterReference.noControl = true;
        ReferenceManager.Instance.characterReference.transform.DOMove(posCharaEnd1.position, durationEnd).SetEase(Ease.Linear);

        ReferenceManager.Instance.characterReference.isCrossingDoor = true;

       isStatic = true;

        yield return new WaitForSeconds(durationEnd * 0.15f);

        transform.DORotate(posCameraEnd.rotation.eulerAngles, durationEnd);
        transform.DOMove(posCameraEnd.position, durationEnd).SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(durationEnd * 0.85f);

        ReferenceManager.Instance.characterReference.isCrossingDoor = false;
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
        lightToActivate = scriptFondu.currentActivatedLight;

        savePosition = transform.position;
        saveRotation = transform.rotation;

        saveMinXZ = minXZ;
        saveMaxXZ = maxXZ;

        saveCameraRefPos = cameraPosRef.position;
    }

    // QUAND ON ARRETE DE CONTROLER UN OBJET
    public void LoadCamPos()
    {
        if(lightToActivate != null && scriptFondu.currentActivatedLight != null)
        {
            scriptFondu.currentActivatedLight.SetActive(false);
            lightToActivate.SetActive(true);

            scriptFondu.currentActivatedLight = lightToActivate;
        }
        
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
