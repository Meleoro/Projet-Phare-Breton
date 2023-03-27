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
    [HideInInspector] public Rigidbody rb;

    [Header("Inputs")]
    [HideInInspector] public Vector2 direction;
    [HideInInspector] public bool R2;
    [HideInInspector] public bool moveObject;
    [HideInInspector] public bool interaction;
    [HideInInspector] public bool escape;

    [Header("Notes")]
    [HideInInspector] public GameObject nearNoteObject;
    [HideInInspector] public int nearNotePartitionNumber;
    [HideInInspector] public int nearNoteNumber;

    [Header("Autres")] public string menuScene;
    [HideInInspector] public bool noMovement;
    [HideInInspector] public bool noControl;
    [HideInInspector] public bool hasRope;
    [HideInInspector] public bool isMovingObjects;
    [HideInInspector] public List<Rigidbody> movedObjects = new List<Rigidbody>();
    [HideInInspector] public List<GameObject> nearObjects = new List<GameObject>();
    [HideInInspector] public List<ObjetInteractible> scriptsMovedObjects = new List<ObjetInteractible>();
    [HideInInspector] public bool nearLadder;
    [HideInInspector] public Vector3 ladderTPPos;


    void Start()
    {
        rb = GetComponent<Rigidbody>();

        hasRope = false;
    }
    
    void Update()
    {
        if (escape)
        {
            escape = false;

            SceneManager.LoadScene(menuScene);
        }
        
        if (!noControl)
        {
            // Partie déplacement player / objets
            if (!noMovement && !isMovingObjects)
            {
                movementScript.MoveCharacter(direction);
                
                if(direction == Vector2.zero)
                    movementScript.RotateCharacter();
            }
            else if (isMovingObjects)
            {
                movementScript.MoveObjects(movedObjects, scriptsMovedObjects, direction);
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


            // Partie flûte 
            if (R2 && !hasRope && !isMovingObjects)
            {
                fluteScript.FluteActive(direction);
                movementScript.RotateCharacter();
                movementScript.MoveCharacter(Vector2.zero);

                if (interaction)
                {
                    interaction = false;
                    fluteScript.CreateLien();
                }
                else if (moveObject)
                {
                    moveObject = false;
                    fluteScript.MoveObject(false, null);
                }
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
        }
    }
    
    
    
    //------------------------------------------------------------------------------------
    // PARTIE INPUT
    
    
    // LE JOUEUR MAINTIENT R2
    public void OnFlute(InputAction.CallbackContext context)
    {
        R2 = context.performed;
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
    
    public void OnEscape(InputAction.CallbackContext context)
    {
        if (context.started)
            escape = true;
        
        if (context.canceled)
            escape = false;
    }
}
