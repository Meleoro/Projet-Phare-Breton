using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    [Header("References")] 
    private CharaManager manager;

    [Header("Movements")]
    [SerializeField, Range(0f, 100f)] float maxSpeed = 10f;
    [SerializeField, Range(0f, 100f)] float maxAcceleration = 10f;
    private Vector3 velocity;

    [Header("MovementsObjets")]
    [SerializeField] private float hauteurObject = 0.5f;
    [SerializeField, Range(0f, 100f)] float maxSpeedObject = 10f;
    [SerializeField, Range(0f, 100f)] float maxAccelerationObject = 10f;
    private Vector3 velocityObject;


    private void Awake()
    {
        manager = GetComponent<CharaManager>();
    }


    //------------------------------------------------------------------------------------------------------------------
    

    // BOUGE LE PERSONNAGE
    public void MoveCharacter(Vector2 direction)
    {
        Vector3 desiredVelocity = new Vector3(direction.x, 0f, direction.y) * maxSpeed;
        
        float maxSpeedChange = maxAcceleration * Time.deltaTime;
        
        // Acceleration du personnage
        velocity = transform.InverseTransformDirection(manager.rb.velocity);
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);
        manager.rb.velocity = transform.TransformDirection(velocity);
    }

    // ORIENTATION DU PERSONNAGE EN FONCTION DE L'ANGLE DE CAMERA
    public void RotateCharacter()
    {
        transform.rotation = Quaternion.Euler(0, ReferenceManager.Instance.cameraReference.transform.rotation.eulerAngles.y, 0);
    }


    // BOUGE LES OBJETS CONTROLES PAS LE JOUEUR
    public void MoveObjects(List<Rigidbody> objects, Vector2 direction)
    {
        for(int k = 0; k < objects.Count; k++)
        {
            Vector3 desiredVelocity = new Vector3(direction.x, 0f, direction.y) * maxSpeedObject;

            float maxSpeedChange = maxAccelerationObject * Time.deltaTime;

            // Acceleration de l'objet
            velocityObject = transform.InverseTransformDirection(objects[k].velocity);
            velocityObject.x = Mathf.MoveTowards(velocityObject.x, desiredVelocity.x, maxSpeedChange);
            velocityObject.z = Mathf.MoveTowards(velocityObject.z, desiredVelocity.z, maxSpeedChange);
            objects[k].velocity = transform.TransformDirection(velocityObject);

            // Levitation de l'objet
            if(objects[k].transform.position.y < transform.position.y + hauteurObject)
            {
                objects[k].AddForce(Vector3.up * Mathf.Lerp(3, 10, transform.position.y + hauteurObject - objects[k].transform.position.y), ForceMode.Force);
            }
            else
            {
                objects[k].AddForce(Vector3.up * Mathf.Lerp(5, 0.3f, -(transform.position.y + hauteurObject - objects[k].transform.position.y)), ForceMode.Force);
            }
        }
    }
}
