using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CharaManager : MonoBehaviour
{
    [Header("Scripts")] 
    public CharacterMovement movementScript;
    public CharacterFlute fluteScript;
    public CharacterNotes notesScript;

    [Header("Références")]
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject UIInteraction;
    [HideInInspector] public Rigidbody rb;

    [Header("Inputs")]
    [HideInInspector] public Vector2 direction;
    [HideInInspector] public bool R2;
    [HideInInspector] public bool moveObject;
    [HideInInspector] public bool interaction;
    [HideInInspector] public bool stase;
    [HideInInspector] public bool escape;

    [Header("Notes")]
    [HideInInspector] public GameObject nearNoteObject;
    [HideInInspector] public int nearNotePartitionNumber;
    [HideInInspector] public int nearNoteNumber;
    [HideInInspector] public bool canPlayMusic;
    [HideInInspector] public int currentMelodyIndex;

    [Header("Animations")]
    private bool isWalking;

    [Header("Autres")] public string menuScene;
    [HideInInspector] public bool noMovement;
    [HideInInspector] public bool noControl;
    [HideInInspector] public bool hasRope;
    [HideInInspector] public bool isMovingObjects;
    [HideInInspector] public List<Rigidbody> movedObjects = new List<Rigidbody>();
    [HideInInspector] public List<GameObject> nearObjects = new List<GameObject>();
    [HideInInspector] public List<ObjetInteractible> scriptsMovedObjects = new List<ObjetInteractible>();
    [HideInInspector] public bool nearLadder;
    [HideInInspector] public bool inJumpZone;
    [HideInInspector] public Vector3 ladderTPPos;
    [HideInInspector] public Vector3 movedObjectPosition;
    [HideInInspector] public bool isInLightSource;


    void Start()
    {
        rb = GetComponent<Rigidbody>();

        hasRope = false;
    }
    
    void Update()
    {
        if(!isMovingObjects)
        {
            movedObjectPosition = transform.position;
            rb.isKinematic = false;
        }

        else
        {
            movedObjectPosition = movedObjects[0].transform.position;
            rb.isKinematic = true;
        }

        

        if (escape)
        {
            escape = false;

            SceneManager.LoadScene(menuScene);
        }
        
        if (!noControl)
        {
            if(interaction && canPlayMusic)
            {
                notesScript.StartPlay(currentMelodyIndex);
            }




            if(nearObjects.Count != 0)
            {
                UIInteraction.SetActive(VerificationInteractionUI());
            }
            else
            {
                UIInteraction.SetActive(false);
            }


            // Partie déplacement player / objets
            if (!noMovement && !isMovingObjects)
            {
                movementScript.MoveCharacter(direction);
                
                if(direction != Vector2.zero) 
                    movementScript.RotateCharacter(direction);

                if (direction == Vector2.zero)
                    movementScript.RotateCharacterCamera();


                if (direction != Vector2.zero)
                    isWalking = true;

                else
                    isWalking = false;
            }

            else if (isMovingObjects)
            {
                movementScript.MoveObjects(movedObjects, scriptsMovedObjects, direction);

                isWalking = false;
            }


            // Interaction
            if(nearObjects.Count > 0 && interaction && !noMovement && !hasRope && !nearLadder)
            {
                // Si c'est une note
                if (nearNotePartitionNumber != 0 && nearNoteNumber != 0)
                {
                    nearObjects.Clear();
                    
                    notesScript.AddNote(nearNotePartitionNumber, nearNoteNumber);

                    Destroy(nearNoteObject);
                    nearNoteNumber = 0;
                    nearNotePartitionNumber = 0;
                }

                else 
                {
                    movementScript.ClimbObject(nearObjects[0]);
                }
                
                interaction = false;
            }

            else if (nearLadder && interaction)
            {
                interaction = false;
                movementScript.ClimbLadder(ladderTPPos);
            }


            // Saut du personnage
            if (interaction)
            {
                if (inJumpZone)
                {
                    movementScript.GoDown();
                }
            }


            // Partie flûte 
            if (R2 && !hasRope)
            {
                movementScript.MoveCharacter(Vector2.zero);
                fluteScript.FluteActive(direction);

                movementScript.RotateCharacterCamera();
                movementScript.RotateCharacter(direction);

                if (interaction && fluteScript.selectedObjects.Count != 0)
                {
                    interaction = false;
                    fluteScript.CreateLien();
                    fluteScript.PlayVFX();
                }
                else if (moveObject && !isMovingObjects && fluteScript.selectedObjects.Count != 0)
                {
                    moveObject = false;
                    fluteScript.MoveObject(false, null);
                    fluteScript.PlayVFX();
                }
                else if (stase && fluteScript.selectedObjects.Count != 0)
                {
                    stase = false;
                    fluteScript.Stase();
                    fluteScript.PlayVFX();
                }

                isWalking = false;
            }
            
            else
            {
                fluteScript.FluteUnactive();
            }

            
            // Partie arrêt des pouvoirs
            if (interaction && hasRope)
            {
                fluteScript.PlaceLien();
            }
            else if(moveObject && isMovingObjects)
            {
                fluteScript.StopMoveObject();
            }

            
            // Pour les effets des contrôles se fasssent qu'une fois
            interaction = false;
            moveObject = false;
            stase = false;
        }
        else
        {
            movementScript.MoveCharacter(Vector2.zero);

            isWalking = false;
        }

        anim.SetBool("isWalking", isWalking);
    }
    
    
    public bool VerificationInteractionUI()
    {
        for (int i = 0; i < nearObjects.Count; i++)
        {
            Boite newObject;
            Echelle newEchelle;

            if(nearObjects[i].TryGetComponent<Boite>(out newObject) || nearObjects[i].TryGetComponent<Echelle>(out newEchelle))
            {
                return true;
            }
        }

        return false;
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
        direction = context.ReadValue<Vector2>();
    }

    public void OnLien(InputAction.CallbackContext context)
    {
        if(context.started)
            interaction = true;
        
        if (context.canceled)
            interaction = false;
    }

    public void OnMoveObject(InputAction.CallbackContext context)
    {
        if (context.started)
            moveObject = true;
        
        if (context.canceled)
            moveObject = false;
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
