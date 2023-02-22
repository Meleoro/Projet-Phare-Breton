using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCharacter : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 velocity;
    
    [SerializeField, Range(0f, 100f)] float maxSpeed = 10f;
    [SerializeField, Range(0f, 100f)] float maxAcceleration = 10f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        
        
        rb.velocity = velocity;
    }
}
