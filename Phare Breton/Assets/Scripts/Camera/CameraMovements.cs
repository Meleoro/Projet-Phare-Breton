using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
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
    [HideInInspector] public Transform posCameraRythme3;
    [HideInInspector] public float durationRythme;
    [HideInInspector] public bool moveCameraRythme;
    private float timerMoveRythme;
    private Vector3 savePosRythme;
    private Quaternion saveRotRythme;
    [HideInInspector] public bool goToSave;
    
    [Header("CinematiqueIntro")]
    public bool doStartCinematique;
    public Transform posStart;
    public float duration;
    
    [Header("CinematiqueMiddle")]
    public bool doMiddleCinematique;
    public Transform posCameraMiddle;
    public Transform posCharaMiddle1;
    public Transform posCharaMiddle2;
    public float durationMiddle;

    [Header("Scene2Middle")] 
    public Transform charaPosMiddleScene2;
    public Transform cameraPosMiddleScnee2;
    
    [Header("Scene3Middle")] 
    public Transform charaPosMiddleScene3;
    public Transform cameraPosMiddleScnee3;

    [Header("CinematiqueFin")]
    public UIFin UIScript;
    public bool doEndCinematique;
    public Transform pivotPlan6;
    public Animator animGrue;
    public AnimationCurve cameraSpeedRotate;
    public Transform posCameraEnd1;
    public Transform posCameraEnd2;
    public Transform posCameraEnd3;
    public Transform posCameraEnd4;
    public Transform posCameraEnd5;
    public Transform posCameraEnd6;
    public Transform posCameraEnd7;
    public Transform posCharaEnd1;
    public Transform posCharaEnd2;
    public Transform posCharaEnd3;
    public Transform posCharaEnd4;
    public float durationEnd1;
    public float durationEnd2;
    public float durationEnd3;
    public float durationEnd4;
    public float durationEnd5;
    public float durationEnd6;
    public float durationEnd7;
    public float durationEnd8;
    public TextMeshProUGUI thankyoutext;

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
    private bool saveStatic;

    private List<TransparencyObject> desactivatedObjects = new List<TransparencyObject>();
    private float currentMinAlphaCamera;
    private float currentMaxAlphaCamera;
    private float currentMinAlphaChara;
    private float currentMaxAlphaChara;
    private bool isShaking;
    private bool isinside;


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
        
        if (doStartCinematique)
        {
            StartCoroutine(IntroCinematique());
        }
        else if (doMiddleCinematique && !SaveManager.Instance.goMiddle) 
        {
            StartCoroutine(MiddleCinematique());
        }
        
        else if (doMiddleCinematique && SaveManager.Instance.goMiddle)
        {
            ReferenceManager.Instance.characterReference.transform.position = charaPosMiddleScene2.transform.position;
            ReferenceManager.Instance.characterReference.canCable = true;
            
            transform.position = cameraPosMiddleScnee2.transform.position;
            transform.rotation = cameraPosMiddleScnee2.transform.rotation;

            SaveManager.Instance.goMiddle = false;
        }
        
        else if (SaveManager.Instance.goMiddle)
        {
            ReferenceManager.Instance.characterReference.transform.position = charaPosMiddleScene3.transform.position;
            ReferenceManager.Instance.characterReference.canStase = true;
            
            transform.position = cameraPosMiddleScnee3.transform.position;
            transform.rotation = cameraPosMiddleScnee3.transform.rotation;
            
            SaveManager.Instance.goMiddle = false;
        }
        
        ActualiseRotationCamRef();

        StartCoroutine(GererAmbiance());
    }


    private void Update()
    {
        if (doEndCinematique)
            StartCoroutine(EndCinematique());
        

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
    
    
    private IEnumerator GererAmbiance()
    {
        if (doStartCinematique)
        {
            AudioManager.instance.PlaySoundOneShot(3, 4, 0, ReferenceManager.Instance.characterReference.ambianceAudioSource);
            
            yield return new WaitForSeconds(2);
            
            //AudioManager.instance.PlaySoundFadingIn(0, 1.5f, 2,4,0, ReferenceManager.Instance.characterReference.ambianceAudioSource);
            
            AudioManager.instance.PlaySoundContinuous(2,4,0, ReferenceManager.Instance.characterReference.ambianceAudioSource);
        }
        
        else if (doMiddleCinematique)
        {
            //AudioManager.instance.PlaySoundFadingIn(0, 1.5f, 0,4,0, ReferenceManager.Instance.characterReference.ambianceAudioSource);
            
            AudioManager.instance.PlaySoundContinuous(0,4,0, ReferenceManager.Instance.characterReference.ambianceAudioSource);
        }
        
        else if(SaveManager.Instance.goMiddle)
        {
            //AudioManager.instance.PlaySoundFadingIn(0, 1.5f, 1,4,0, ReferenceManager.Instance.characterReference.ambianceAudioSource);

            AudioManager.instance.PlaySoundContinuous(1,4,0, ReferenceManager.Instance.characterReference.ambianceAudioSource);
        }
        else
        {
            isinside = true;
        }
    
    }

    public void Alternate()
    {
        isinside = !isinside;

        if (isinside && doMiddleCinematique)
        {
            AudioManager.instance.FadeOutAudioSource(0.15f, 0.15f, 0, ReferenceManager.Instance.characterReference.ambianceAudioSource);
        }
        else if(!isinside && doMiddleCinematique)
        {
            AudioManager.instance.PlaySoundContinuous(0,4,0, ReferenceManager.Instance.characterReference.ambianceAudioSource);
        }
        
        else if (isinside && !doStartCinematique)
        {
            AudioManager.instance.FadeOutAudioSource(0.15f, 0.15f, 0, ReferenceManager.Instance.characterReference.ambianceAudioSource);
        }
        else if (!isinside && !doStartCinematique)
        {
            AudioManager.instance.PlaySoundContinuous(1,4,0, ReferenceManager.Instance.characterReference.ambianceAudioSource);
        }
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
        doEndCinematique = false;

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
        doEndCinematique = false;

        isStatic = true;
        ReferenceManager.Instance.characterReference.noControl = true;
        ReferenceManager.Instance.characterReference.rb.isKinematic = true;

        AudioManager.instance.PlaySoundFadingIn(0.15f, 0.15f, 4, 2, 0, ReferenceManager.Instance.characterReference.musicAudioSource);

        yield return new WaitForSeconds(0.02f);


        // PLAN 1
        ReferenceManager.Instance.characterReference.transform.DOMove(posCharaEnd1.position, 0);

        transform.DOMove(posCameraEnd1.position, 0);
        transform.DORotate(posCameraEnd1.rotation.eulerAngles, 0);

        yield return new WaitForSeconds(durationEnd1);


        // PLAN 2
        transform.DOMove(posCameraEnd2.position, durationEnd2 * 0.8f).SetEase(Ease.InOutSine);
        transform.DORotate(posCameraEnd2.rotation.eulerAngles, durationEnd2 * 0.8f).SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(durationEnd2 * 0.2f);

        ReferenceManager.Instance.characterReference.isCrossingDoor = true;
        ReferenceManager.Instance.characterReference.transform.DOMove(posCharaEnd2.position, durationEnd2 * 0.6f).SetEase(Ease.Linear);

        yield return new WaitForSeconds(durationEnd2 * 0.6f);

        ReferenceManager.Instance.characterReference.isCrossingDoor = false;

        yield return new WaitForSeconds(durationEnd2 * 0.4f + 0.01f);


        // PLAN 3
        /*transform.DOMove(posCameraEnd3.position, 0);
        transform.DORotate(posCameraEnd3.rotation.eulerAngles, 0);

        ReferenceManager.Instance.characterReference.isCrossingDoor = true;
        ReferenceManager.Instance.characterReference.transform.DOMove(posCharaEnd3.position, durationEnd3 * 0.5f).SetEase(Ease.Linear);

        yield return new WaitForSeconds(durationEnd3 * 0.5f);

        ReferenceManager.Instance.characterReference.isCrossingDoor = false;

        yield return new WaitForSeconds(durationEnd3 * 0.5f);*/


        // PLAN 4
        ReferenceManager.Instance.characterReference.end = true;

        transform.DOMove(posCameraEnd4.position, 0);
        transform.DORotate(posCameraEnd4.rotation.eulerAngles, 0);
        
        yield return new WaitForSeconds(0.01f);

        transform.DOMove(posCameraEnd5.position, durationEnd4);
        transform.DORotate(posCameraEnd5.rotation.eulerAngles, durationEnd4);

        ReferenceManager.Instance.characterReference.isCrossingDoor = true;
        ReferenceManager.Instance.characterReference.transform.DOMove(posCharaEnd4.position, durationEnd4).SetEase(Ease.Linear);

        yield return new WaitForSeconds(durationEnd4 + 0.01f);

        ReferenceManager.Instance.characterReference.isCrossingDoor = false;


        // PLAN 5
        //Il me manque l'animation donc je skip


        // PLAN 6
        transform.DOMove(posCameraEnd6.position, 0);
        transform.DORotate(posCameraEnd6.rotation.eulerAngles, 0);
        
        ReferenceManager.Instance.characterReference.anim.SetTrigger("end");
        animGrue.SetTrigger("startEnd");

        yield return new WaitForSeconds(durationEnd5);


        // PLAN 7
        transform.parent = pivotPlan6;

        //pivotPlan6.DORotate(pivotPlan6.rotation.eulerAngles + new Vector3(-40, 180, 0), durationEnd6 * 0.5f).SetEase(Ease.Linear);


        StartCoroutine(RotateCamera(durationEnd6 * 0.5f, pivotPlan6.rotation.eulerAngles, pivotPlan6.rotation.eulerAngles + new Vector3(-45, -179, 0), false));

        yield return new WaitForSeconds(durationEnd6 * 0.51f);

        ReferenceManager.Instance.characterReference.movementScript.mesh.gameObject.SetActive(false);

        StartCoroutine(RotateCamera(durationEnd6 * 0.5f, pivotPlan6.rotation.eulerAngles, pivotPlan6.rotation.eulerAngles + new Vector3(30, -90, 0), true));

        pivotPlan6.DOMove(pivotPlan6.transform.position + new Vector3(0, 0, 2), durationEnd6 * 0.5f).SetEase(Ease.Linear);

        yield return new WaitForSeconds(durationEnd6 * 0.6f);

        transform.parent = parentTranform;


        // PLAN 8
        transform.DOMove(posCameraEnd6.position, 0);
        transform.DORotate(posCameraEnd6.rotation.eulerAngles, 0);

        yield return new WaitForSeconds(durationEnd7 * 0.2f);

        animGrue.SetTrigger("startVol");

        yield return new WaitForSeconds(0.9f);

        Vector3 wantedPos = animGrue.transform.position + animGrue.transform.forward * 2 + animGrue.transform.up * 40;

        animGrue.transform.DOMoveX(wantedPos.x, durationEnd7).SetEase(Ease.Linear);
        animGrue.transform.DOMoveZ(wantedPos.z, durationEnd7).SetEase(Ease.Linear);

        animGrue.transform.DOMoveY(wantedPos.y, durationEnd7);

        yield return new WaitForSeconds(durationEnd7);


        // PLAN 9
        transform.DOMove(posCameraEnd7.position, durationEnd8);
        transform.DORotate(posCameraEnd7.rotation.eulerAngles, durationEnd8);

        yield return new WaitForSeconds(durationEnd8);

        StartCoroutine(UIScript.Credits());



        /*transform.DOMove(posCameraEnd2.position, durationEnd2).SetEase(Ease.InOutSine);
        transform.DORotate(posCameraEnd2.rotation.eulerAngles, durationEnd2).SetEase(Ease.InOutSine);

        ReferenceManager.Instance.characterReference.anim.SetTrigger("end");*/
    }


    public IEnumerator RotateCamera(float totalDuration, Vector3 rotStart, Vector3 rotWanted, bool inverse)
    {
        if (!inverse)
        {
            float currentDuration = totalDuration;

            while(currentDuration > 0)
            {
                float ratio = 1 - (currentDuration / totalDuration);

                pivotPlan6.rotation = Quaternion.Lerp(Quaternion.Euler(rotStart), Quaternion.Euler(rotWanted), cameraSpeedRotate.Evaluate(ratio));

                yield return new WaitForEndOfFrame();

                currentDuration -= Time.deltaTime;
            }
        }
        else
        {
            float currentDuration = 0;

            while(currentDuration < totalDuration)
            {
                float ratio = currentDuration / totalDuration;

                pivotPlan6.rotation = Quaternion.Lerp(Quaternion.Euler(rotStart), Quaternion.Euler(rotWanted), cameraSpeedRotate.Evaluate(ratio));

                yield return new WaitForEndOfFrame();

                currentDuration += Time.deltaTime;
            }
        }

        /*ReferenceManager.Instance.characterReference.movementScript.mesh.gameObject.SetActive(false);

        while (currentDuration < 1)
        {
            currentDuration += Time.deltaTime;
            float ratio = (currentDuration / totalDuration);

            pivotPlan6.rotation = Quaternion.Lerp(Quaternion.Euler(rotWanted), Quaternion.Euler(rotWanted2), cameraSpeedRotate.Evaluate(ratio));

            yield return new WaitForSeconds(Time.deltaTime);
        }*/
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

                Vector3 wantedPos = Vector3.Lerp(posCameraRythme.position, posCameraRythme2.position,  (avancee - depart) * 2);
                Quaternion wanterRot = Quaternion.Lerp(posCameraRythme.rotation, posCameraRythme2.rotation, (avancee - depart) * 2);
                
                if ((avancee - depart) * 2 > 1)
                {
                    wantedPos = Vector3.Lerp(posCameraRythme2.position, posCameraRythme3.position, (avancee - depart) * 2 - 1);
                    wanterRot = Quaternion.Lerp(posCameraRythme2.rotation, posCameraRythme3.rotation, (avancee - depart) * 2 - 1);
                }

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

        saveStatic = isStatic;
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
        
        isStatic = saveStatic;

        
        lightToActivate = null;
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
