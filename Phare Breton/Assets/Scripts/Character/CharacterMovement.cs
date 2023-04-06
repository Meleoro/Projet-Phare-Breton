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
    [HideInInspector] public bool doOnce;
    [HideInInspector] public Vector3 resistanceCable;

    [Header("MovementsObjets")]
    public float hauteurObject = 0.5f;
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

        Vector3 newResistance = ReferenceManager.Instance.cameraRotationReference.transform.InverseTransformDirection(resistanceCable);
        desiredVelocity += new Vector3(newResistance.x, 0, newResistance.z) * maxSpeed;
        
        float maxSpeedChange = maxAcceleration * Time.deltaTime;

        // Acceleration du personnage
        velocity = ReferenceManager.Instance.cameraRotationReference.transform.InverseTransformDirection(manager.rb.velocity);
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);
        manager.rb.velocity = ReferenceManager.Instance.cameraRotationReference.transform.TransformDirection(velocity);
    }

    // ORIENTATION DU PERSONNAGE EN FONCTION DE L'ANGLE DE CAMERA
    public void RotateCharacter()
    {
        ReferenceManager.Instance.cameraReference.ActualiseRotationCamRef();
    }


    // BOUGE LES OBJETS CONTROLES PAS LE JOUEUR
    public void MoveObjects(List<Rigidbody> objects, List<ObjetInteractible> scripts, Vector2 direction)
    {
        for(int k = 0; k < objects.Count; k++)
        {
            Vector3 desiredVelocity = new Vector3(direction.x, 0f, direction.y) * maxSpeedObject;

            float maxSpeedChange = maxAccelerationObject * Time.deltaTime;

            // Acceleration de l'objet
            velocityObject = ReferenceManager.Instance.cameraRotationReference.transform.InverseTransformDirection(objects[k].velocity);
            velocityObject.x = Mathf.MoveTowards(velocityObject.x, desiredVelocity.x, maxSpeedChange);
            velocityObject.z = Mathf.MoveTowards(velocityObject.z, desiredVelocity.z, maxSpeedChange);
            objects[k].velocity = ReferenceManager.Instance.cameraRotationReference.transform.TransformDirection(velocityObject);


            if (!scripts[k].isMagneted)
            {
                // Levitation de l'objet
                if(objects[k].transform.position.y < scripts[k].currentHauteur)
                {
                    objects[k].AddForce(Vector3.up * 8, ForceMode.Acceleration);
                }
                else
                {
                    objects[k].transform.position = new Vector3(objects[k].transform.position.x, scripts[k].currentHauteur, objects[k].transform.position.z);
                }
            }
        }
    }


    // PLACE LE JOUEUR AU DESSUS D'UN OBJET
    public void ClimbObject(GameObject climbedObject)
    {
        if(climbedObject.GetComponent<ObjetInteractible>().isClimbable)
            transform.position = climbedObject.transform.position + Vector3.up * 2;
    }
    

    // PERMET AU JOUEUR D'UTILISER UNE ECHELLE
    public void ClimbLadder(Vector3 newPos)
    {
        transform.position = newPos;
    }
}
