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
    private Vector2 direction;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    void Update()
    {
        movementScript.MoveCharacter(direction);
        movementScript.RotateCharacter();
    }
    
    
    
    //------------------------------------------------------------------------------------
    // PARTIE INPUT
    
    
    // LE JOUEUR MAINTIENT R2
    public void OnFlute(InputValue value)
    {
        //value.
    }
    
    //RECUPERE INPUT DE DIRECTION
    public void OnDirection(InputValue value)
    {
        direction = value.Get<Vector2>();
    }
}
