using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private Transform mesh;
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

    [Header("Fall")] 
    private Vector2 stockageDirection;


    private void Awake()
    {
        manager = GetComponent<CharaManager>();
    }


    //------------------------------------------------------------------------------------------------------------------
    

    // BOUGE LE PERSONNAGE
    public void MoveCharacter(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            stockageDirection = direction;
        }

        bool willFall = VerifyFall(direction);

        if (!willFall)
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
        else
        {
            manager.rb.velocity = Vector3.zero;
        }
    }

    // ORIENTATION LES CONTROLES DU PERSONNAGE EN FONCTION DE L'ANGLE DE CAMERA
    public void RotateCharacterCamera()
    {
        ReferenceManager.Instance.cameraReference.ActualiseRotationCamRef();
    }

    // ORIENTE LE MESH DU PERSONNAGE
    public void RotateCharacter(Vector2 direction)
    {
        Vector3 newDirection = ReferenceManager.Instance.cameraRotationReference.transform.TransformDirection(new Vector3(direction.x, 0, direction.y));

        mesh.rotation = Quaternion.LookRotation(newDirection, Vector3.up) * Quaternion.Euler(0, 180, 0);
    }


    // BOUGE LES OBJETS CONTROLES PAS LE JOUEUR
    public void MoveObjects(List<Rigidbody> objects, List<ObjetInteractible> scripts, Vector2 direction)
    {
        for(int k = 0; k < objects.Count; k++)
        {
            if (!scripts[k].isMagneted)
            {
                if (!ReferenceManager.Instance.cameraReference.scriptFondu.isInTransition)
                {
                    // Levitation de l'objet
                    if (objects[k].transform.position.y < scripts[k].currentHauteur)
                    {
                        objects[k].AddForce(Vector3.up * 1000 * Time.deltaTime, ForceMode.Acceleration);
                    }
                    else
                    {
                        objects[k].transform.position = new Vector3(objects[k].transform.position.x, scripts[k].currentHauteur, objects[k].transform.position.z);
                    }
                }
            }


            Vector3 desiredVelocity = new Vector3(direction.x, 0f, direction.y) * maxSpeedObject;

            float maxSpeedChange = maxAccelerationObject * Time.deltaTime;

            // Acceleration de l'objet
            velocityObject = ReferenceManager.Instance.cameraRotationReference.transform.InverseTransformDirection(objects[k].velocity);
            velocityObject.x = Mathf.MoveTowards(velocityObject.x, desiredVelocity.x, maxSpeedChange);
            velocityObject.z = Mathf.MoveTowards(velocityObject.z, desiredVelocity.z, maxSpeedChange);
            objects[k].velocity = ReferenceManager.Instance.cameraRotationReference.transform.TransformDirection(velocityObject);
        }
    }


    // PLACE LE JOUEUR AU DESSUS D'UN OBJET
    public void ClimbObject(GameObject climbedObject)
    {
        if (climbedObject.GetComponent<ObjetInteractible>().isClimbable)
        {
            if (VerifyFall(stockageDirection))
            {
                transform.position = CalculateFallPos(stockageDirection);
            }
            else
            {
                transform.position = climbedObject.transform.position + Vector3.up * 2;
            }
        }
    }

    public void GoDown()
    {
        if (VerifyFall(stockageDirection))
        {
            transform.position = CalculateFallPos(stockageDirection);
        }
    }
    

    // PERMET AU JOUEUR D'UTILISER UNE ECHELLE
    public void ClimbLadder(Vector3 newPos)
    {
        transform.position = newPos;
    }


    public bool VerifyFall(Vector2 direction)
    {
        Vector3 point1 = Vector3.zero;
        Vector3 point2 = new Vector3(0, -5000, 0);
        Vector3 point3 = new Vector3(0, -5000, 0);


        // Raycast 1
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit raycastHit;
        
        if(Physics.Raycast(ray, out raycastHit, 5))
        {
            point1 = raycastHit.point;
        }
        
        
        // Raycast 2
        Vector3 direction2 = ReferenceManager.Instance.cameraRotationReference.transform.TransformDirection(new Vector3(direction.x, 0, direction.y));
        
        ray = new Ray(transform.position + (direction2 * 0.8f), Vector3.down);
        RaycastHit raycastHit2;

        if(Physics.Raycast(ray, out raycastHit2, 5))
        {
            point2 = raycastHit2.point;
        }


        // Raycast3 (pentes)
        ray = new Ray(transform.position + (direction2 * 0.4f), Vector3.down);
        RaycastHit raycastHit3;

        if (Physics.Raycast(ray, out raycastHit3, 5))
        {
            point3 = raycastHit3.point;
        }



        float difference1 = point1.y - point2.y;
        float difference2 = point1.y - point3.y;

        float difference3 = point3.y - point2.y;

        if (difference2 > 0.1f && difference3 + 0.8f > difference2)
        {
            return false;
        }


        if (difference1 > 0.5f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }



    public Vector3 CalculateFallPos(Vector2 direction)
    {
        Vector3 posFall = Vector3.zero;
        Vector3 direction2 = ReferenceManager.Instance.cameraRotationReference.transform.TransformDirection(new Vector3(direction.x, 0, direction.y));
        
        Ray ray = new Ray(transform.position + direction2, Vector3.down);
        RaycastHit raycastHit2;

        if(Physics.Raycast(ray, out raycastHit2, 5))
        {
            posFall = raycastHit2.point + Vector3.up;
        }
        
        return posFall;
    }
    
}
