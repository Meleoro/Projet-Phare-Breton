using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharaManager : MonoBehaviour
{
    [Header("Scripts")] 
    public CharacterMovement movementScript;
    public CharacterFlute fluteScript;
    
    [Header("Références")]
    [HideInInspector] public Rigidbody rb;

    [Header("Inputs")]
    [HideInInspector] public Vector2 direction;
    [HideInInspector] public bool R2;
    [HideInInspector] public bool lien;
    [HideInInspector] public bool moveObject;
    [HideInInspector] public bool interaction;

    [Header("Autres")] 
    [HideInInspector] public bool noMovement;
    [HideInInspector] public bool noControl;
    [HideInInspector] public bool hasRope;
    [HideInInspector] public bool isMovingObjects;
    [HideInInspector] public List<Rigidbody> movedObjects = new List<Rigidbody>();
    [HideInInspector] public List<GameObject> nearObjects = new List<GameObject>();


    void Start()
    {
        rb = GetComponent<Rigidbody>();

        hasRope = false;
    }
    
    void Update()
    {
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
                movementScript.MoveObjects(movedObjects, direction);
            }


            // Escalade
            if(nearObjects.Count > 0 && interaction && !noMovement)
            {
                movementScript.ClimbObject(nearObjects[0]);
            }

            
            // Partie flûte 
            if (R2 && !hasRope && !isMovingObjects)
            {
                fluteScript.FluteActive(direction);
                movementScript.RotateCharacter();
                movementScript.MoveCharacter(Vector2.zero);

                if (lien)
                {
                    fluteScript.CreateLien();
                }
                else if (moveObject)
                {
                    fluteScript.MoveObject();
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
            else if(interaction && isMovingObjects)
            {
                fluteScript.StopMoveObject();
            }
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
            lien = true;
    }

    public void OnMoveObject(InputAction.CallbackContext context)
    {
        if (context.started)
            moveObject = true;
    }

    public void OnInteraction(InputAction.CallbackContext context)
    {
        if(context.started)
            interaction = true;

        if (context.canceled)
            interaction = false;
    }



    //------------------------------------------------------------------------------------
    // PARTIE DETECTION INTERACTIBLES

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactible"))
        {
            if(other.GetComponent<ObjetInteractible>().isClimbable)
                nearObjects.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactible"))
        {
            if (other.GetComponent<ObjetInteractible>().isClimbable)
                nearObjects.Remove(other.gameObject);
        }
    }
}
