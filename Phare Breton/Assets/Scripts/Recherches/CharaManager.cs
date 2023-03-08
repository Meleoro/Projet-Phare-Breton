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
    [HideInInspector] public bool interaction;

    [Header("Autres")] 
    [HideInInspector] public bool noMovement;
    [HideInInspector] public bool noControl;
    [HideInInspector] public bool hasRope;


    void Start()
    {
        rb = GetComponent<Rigidbody>();

        hasRope = false;
    }
    
    void Update()
    {
        if (!noControl)
        {
            if (!noMovement)
            {
                movementScript.MoveCharacter(direction);
                
                if(direction == Vector2.zero)
                    movementScript.RotateCharacter();
            }

            
            if (R2 && !hasRope)
            {
                fluteScript.FluteActive(direction);
                movementScript.RotateCharacter();
                movementScript.MoveCharacter(Vector2.zero);

                if (lien)
                {
                    fluteScript.CreateLien();
                }
            }
            
            else
            {
                fluteScript.FluteUnactive();
            }

            
            if (interaction && hasRope)
            {
                fluteScript.PlaceLien();
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
    
    public void OnInteraction(InputAction.CallbackContext context)
    {
        if(context.started)
            interaction = true;

        if (context.canceled)
            interaction = false;
    }
}
