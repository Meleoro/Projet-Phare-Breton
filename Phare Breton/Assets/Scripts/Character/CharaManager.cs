    using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CharaManager : MonoBehaviour
{
    [Header("Scripts")] 
    public CharacterMovement movementScript;
    public CharacterFlute fluteScript;
    public CharacterNotes notesScript;
    public CharacterObjects scriptObjets;

    [Header("Références")]
    public Animator anim;
    [SerializeField] private GameObject UIInteraction;
    [SerializeField] private Image UIImageX;
    [SerializeField] private Image UIImageY;
    public TextMeshProUGUI textInteraction;
    [SerializeField] private MeshRenderer fluteMesh;
    [HideInInspector] public Rigidbody rb;
    public AudioSource playerAudioSource;
    public AudioSource ambianceAudioSource;
    public AudioSource musicAudioSource;
    public ParticleSystem waterParticles;

    [Header("Inputs")]
    [HideInInspector] public Vector2 direction;
    [HideInInspector] public Vector2 wantedDirection;
    [HideInInspector] public bool R2;
    [HideInInspector] public bool moveObject;
    [HideInInspector] public bool interaction;
    [HideInInspector] public bool cable;
    [HideInInspector] public bool stase;
    [HideInInspector] public bool escape;

    [Header("Notes")]
    [HideInInspector] public GameObject nearNoteObject;
    [HideInInspector] public int nearNotePartitionNumber;
    [HideInInspector] public int nearNoteNumber;
    [HideInInspector] public bool canPlayMusic;
    [HideInInspector] public int currentMelodyIndex;
    [HideInInspector] public bool isInBiblio;
    [HideInInspector] public List<GameObject> tagToActivate = new List<GameObject>();

    [Header("Powers")]
    public bool canMoveObjects;
    public bool canCable;
    public bool canStase;

    [Header("Animations")]
    [HideInInspector] public bool isWalking;
    [HideInInspector] private bool fluteActive;

    [Header("Selection")]
    [HideInInspector] public List<GameObject> nearObjects = new List<GameObject>();
    [HideInInspector] public List<GameObject> nearBoxes = new List<GameObject>();
    [HideInInspector] public List<GameObject> nearBoxesUp = new List<GameObject>();
    [HideInInspector] public List<GameObject> nearBoxesDown = new List<GameObject>();
    [HideInInspector] public List<GameObject> nearGenerator = new List<GameObject>();
    [HideInInspector] public List<GameObject> nearAmpoule = new List<GameObject>();
    [HideInInspector] public List<GameObject> nearObjetsRecuperables = new List<GameObject>();
    [HideInInspector] public List<GameObject> cableObjects = new List<GameObject>();
    [HideInInspector] public GameObject cableObject;
    [HideInInspector] public Echelle nearLadder;
    public GameObject objectOn;

    [Header("Autres")] 
    public string menuScene;
    public bool raycastDetection;
    [HideInInspector] public bool noMovement;
    [HideInInspector] public bool noControl;
    [HideInInspector] public bool hasRope;
    [HideInInspector] public bool isMovingObjects;
    [HideInInspector] public List<Rigidbody> movedObjects = new List<Rigidbody>();
    [HideInInspector] public List<ObjetInteractible> scriptsMovedObjects = new List<ObjetInteractible>();
    [HideInInspector] public bool inJumpZone;
    [HideInInspector] public Vector3 movedObjectPosition;
    public bool isInLightSource;
    [HideInInspector] public bool isPickingObjectUp;
    [HideInInspector] public bool isCrossingDoor;
    private bool UIActive;
    [HideInInspector] public bool end;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerAudioSource = GetComponent<AudioSource>();

        hasRope = false;
        UIActive = false;

        StartAudio();
    }

    private void StartAudio()
    {
        AudioManager.instance.audioSources.Clear();
        AudioManager.instance.audioSources.Add(playerAudioSource);
        AudioManager.instance.audioSources.Add(musicAudioSource);
        AudioManager.instance.audioSources.Add(ambianceAudioSource);

        AudioManager.instance.musicAudioSources.Clear();
        AudioManager.instance.musicAudioSources.Add(musicAudioSource);

        AudioManager.instance.sfxAudioSources.Clear();
        AudioManager.instance.sfxAudioSources.Add(playerAudioSource);
        AudioManager.instance.sfxAudioSources.Add(ambianceAudioSource);
    }

    
    void Update()
    {
        Cursor.visible = false;
        
        direction = Vector2.Lerp(direction, wantedDirection, Time.deltaTime * 50);

        IsMovingObjects();


        /*if (raycastDetection)
        {
            RaycastHit _raycastHit;
        
            if (Physics.Raycast(transform.position, movementScript.mesh.forward, out _raycastHit,2))
            {
                Debug.Log(_raycastHit.collider.gameObject);
            
                Debug.DrawLine(_raycastHit.point, transform.position, Color.blue, 2);
            }
        }*/


        if (escape)
        {
            escape = false;

            SceneManager.LoadScene(menuScene);
        }
        
        if (!noControl && !isPickingObjectUp)
        {
            fluteMesh.enabled = false;

            if (interaction && canPlayMusic)
            {
                notesScript.StartPlay(currentMelodyIndex);
            }


            // Partie flute
            if (R2)
            {
                FlutePart();
            }

            else
            {
                fluteActive = false;
                fluteScript.FluteUnactive();
            }
            
            
            // Interaction
            if (!fluteActive)
            {
                DisplayUI();
                
                if (interaction)
                {
                    if (nearObjetsRecuperables.Count != 0)
                    {
                        scriptObjets.PickUpObject(nearObjetsRecuperables[0]);
                        
                        interaction = false;
                    }
                    
                    if (nearNotePartitionNumber != 0 && nearNoteNumber != 0)
                    {
                        nearObjects.Clear();

                        AudioManager.instance.PlaySoundOneShot(9, 1, 0, playerAudioSource);

                        notesScript.AddNote(nearNotePartitionNumber, nearNoteNumber);

                        if(isInBiblio)
                            BarresManager.Instance.MoveBarres();
                        
                        Destroy(nearNoteObject);
                        nearNoteNumber = 0;
                        nearNotePartitionNumber = 0;
                        isInBiblio = false;
                    }
                    
                    if(nearObjects.Count > 0 && !noMovement && !hasRope && nearLadder == null && !isMovingObjects)
                    {
                        movementScript.ClimbObject(nearBoxes);

                        interaction = false;
                    }
                    
                    else if (nearLadder != null)
                    {
                        interaction = false;
                        
                        nearLadder.TakeLadder(transform);
                    }
                }

                // Saut du personnage
                if (interaction)
                {
                    if (inJumpZone)
                    {
                        movementScript.GoDown();
                    }
                }
            }


            // Partie arrêt des pouvoirs
            if (cable && hasRope)
            {
                fluteScript.PlaceLien();
            }
            else if(moveObject && isMovingObjects)
            {
                fluteScript.StopMoveObject();
            }

            if (isMovingObjects)
            {
                if (canStase && stase)
                {
                    List<ObjetInteractible> objectsToStase = new List<ObjetInteractible>();
                    for (int i = 0; i < movedObjects.Count; i++)
                    {
                        objectsToStase.Add(movedObjects[i].GetComponent<ObjetInteractible>());
                    }
                    
                    fluteScript.selectedObjects = objectsToStase;
                    fluteScript.Stase();
                    
                    fluteScript.selectedObjects.Clear();
                    
                    fluteScript.StopMoveObject();
                }
            }


            // Pour les effets des contrôles se fasssent qu'une fois
            interaction = false;
            cable = false;
            moveObject = false;
            stase = false;
        }
        else
        {
            if(isPickingObjectUp)
                scriptObjets.ControlObject(direction, stase);

            if (!isCrossingDoor)
            {
                if (isWalking && end)
                {
                    waterParticles.Stop();
                }

                isWalking = false;
            }
            else
            {
                if (!isWalking && end)
                {
                    waterParticles.Play();
                }

                if (end)
                {
                    movementScript.isOnWater = true;
                    movementScript.PlaySoundPas();
                }

                isWalking = true;
            }

            /*movementScript.MoveCharacter(Vector2.zero);
            movementScript.MoveObjects(movedObjects, scriptsMovedObjects, Vector2.zero);*/
        }

        anim.SetBool("isWalking", isWalking);
        anim.SetBool("fluteActive", fluteActive);
        
        IsMovingObjects();
    }

    private void FixedUpdate()
    {
        if (!noControl && !isPickingObjectUp)
        {
            // Partie déplacement player / objets
            if (!noMovement && !isMovingObjects)
            {
                movementScript.MoveCharacter(direction);
                
                if(direction != Vector2.zero) 
                    movementScript.RotateCharacter(direction, false, false);

                if (direction == Vector2.zero)
                    movementScript.RotateCharacterCamera();


                if (direction.magnitude > 0.2f)
                    isWalking = true;

                else
                    isWalking = false;
            }

            else if (isMovingObjects)
            {
                movementScript.MoveObjects(movedObjects, scriptsMovedObjects, direction);

                isWalking = false;
            }
        }
        else
        {
            movementScript.MoveCharacter(Vector2.zero);
            movementScript.MoveObjects(movedObjects, scriptsMovedObjects, Vector2.zero);
        }
    }


    public void StartCinematique()
    {
        noControl = true;

        anim.SetTrigger("wakeUp");
    }

    public void EndCinematique()
    {
        noControl = false;
    }


    public void DisplayUI()
    {
        bool afficher = false;

        if (nearObjects.Count != 0 || nearNoteNumber != 0 || canPlayMusic || nearObjetsRecuperables.Count != 0 || cableObject != null)
        {
            afficher = true;

            //UIInteraction.SetActive(VerificationInteractionUI());
        }
        /*else
        {
            UIInteraction.SetActive(false);
        }*/

        float speed = 0.2f;

        if (afficher)
        {
            UIInteraction.SetActive(VerificationInteractionUI());

            if (!UIActive)
            {
                UIActive = true;

                UIImageX.DOFade(1, speed).OnComplete(() => UIInteraction.SetActive(true));
                UIImageY.DOFade(1, speed);

                textInteraction.DOFade(1, speed);
            }
        }

        else
        {
            if (UIActive)
            {
                UIActive = false;

                UIImageX.DOFade(0, speed).OnComplete(() => UIInteraction.SetActive(false));
                UIImageY.DOFade(0, speed);

                textInteraction.DOFade(0, speed);
            }
        }
    }


    public bool VerificationInteractionUI()
    {
        if (!hasRope)
        {
            UIImageX.enabled = true;
            UIImageY.enabled = false;
            
            if (nearLadder != null)
                return true;

            if (nearBoxesUp.Count != 0)
                return true;

            if (nearNoteNumber != 0)
                return true;

            if (canPlayMusic)
                return true;

            if (nearObjetsRecuperables.Count != 0)
                return true;
        
            for (int i = 0; i < nearBoxesDown.Count; i++)
            {
                if (movementScript.VerifyFall(movementScript.stockageDirection, true))
                    return true;
            }
        }
        
        else
        {
            UIImageX.enabled = false;
            UIImageY.enabled = true;

            if (nearLadder != null)
            {
                UIImageX.enabled = true;
                UIImageY.enabled = false;

                return true;
            }

            if (cableObject != null)
                return true;
        }
        
        return false;
    }


    public void FlutePart()
    {
        if (canMoveObjects)
        {
            if (!hasRope)
            {
                interaction = false;
                
                movementScript.MoveCharacter(Vector2.zero);
                fluteScript.FluteActive(direction);

                fluteMesh.enabled = true;

                movementScript.RotateCharacterCamera();
                movementScript.RotateCharacter(direction, false, false);

                if (cable && fluteScript.selectedObjects.Count != 0 && canCable)
                {
                    cable = false;
                    R2 = false;
                    fluteScript.CreateLien();
                    fluteScript.PlayVFX();
                }
                else if (moveObject && !isMovingObjects && fluteScript.selectedObjects.Count != 0 && canMoveObjects)
                {
                    moveObject = false;
                    R2 = false;
                    fluteScript.MoveObject(false, null);
                    fluteScript.PlayVFX();
                }
                else if (stase && fluteScript.selectedObjects.Count != 0 && canStase)
                {
                    stase = false;
                    R2 = false;
                    fluteScript.Stase();
                    fluteScript.PlayVFX();
                }

                if (!fluteActive)
                {
                    fluteActive = true;
                    anim.SetTrigger("startFlute");
                }
                isWalking = false;
            }
        }

        else
        {
            R2 = false;

            StartCoroutine(SayNo());
        }
    }
    

    public IEnumerator SayNo()
    {
        anim.SetTrigger("no");
        noControl = true;
        
        yield return new WaitForSeconds(1f);

        noControl = false;
    }

    
    public void IsMovingObjects()
    {
        if(!isMovingObjects)
        {
            movedObjectPosition = transform.position;

            if(!noControl)
                rb.isKinematic = false;
        }

        else
        {
            movedObjectPosition = movedObjects[0].transform.position;

            if(!noControl)
                rb.isKinematic = true;
            
        }
    }



    
    //------------------------------------------------------------------------------------
    // PARTIE INPUT
    
    
    // LE JOUEUR MAINTIENT R2
    public void OnFlute(InputAction.CallbackContext context)
    {
        if (context.started)
            R2 = true;
        
        if (context.canceled)
            R2 = false;
    }

    //RECUPERE INPUT DE DIRECTION
    public void OnDirection(InputAction.CallbackContext context)
    {
        wantedDirection = context.ReadValue<Vector2>();
    }

    public void OnLien(InputAction.CallbackContext context)
    {
        if (context.started)
            cable = true;

        if (context.canceled)
            cable = false;
    }

    public void OnMoveObject(InputAction.CallbackContext context)
    {
        if (context.started)
            moveObject = true;
        
        if (context.canceled)
            moveObject = false;


        if (context.started)
            interaction = true;

        if (context.canceled)
            interaction = false;
    }

    public void OnStase(InputAction.CallbackContext context)
    {
        if (context.started)
            stase = true;

        if (context.canceled)
            stase = false;
    }
    
    public void OnEscape(InputAction.CallbackContext context)
    {
        if (context.started)
            escape = true;
        
        if (context.canceled)
            escape = false;
    }
}
