using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovementR : MonoBehaviour
{
    [Header("References")] 
    private Rigidbody rb;

    [Header("Movements")]
    [SerializeField, Range(0f, 100f)] float maxSpeed = 10f;
    [SerializeField, Range(0f, 100f)] float maxAcceleration = 10f;
    private Vector2 direction;
    private Vector3 velocity;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        MoveCharacter();
    }

    
    //------------------------------------------------------------------------------------------------------------------
    

    // BOUGE LE PERSONNAGE
    private void MoveCharacter()
    {
        Vector3 desiredVelocity = new Vector3(direction.x, 0f, direction.y) * maxSpeed;
        
        float maxSpeedChange = maxAcceleration * Time.deltaTime;
        
        // Acceleration du personnage
        velocity = rb.velocity;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);
        rb.velocity = velocity;
    }

    //RECUPERE INPUT DE DIRECTION
    private void OnDirection(InputValue value)
    {
        direction = value.Get<Vector2>();
    }
}
