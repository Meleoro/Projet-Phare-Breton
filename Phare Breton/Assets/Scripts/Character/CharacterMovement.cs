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
    [SerializeField] private LayerMask layerFall;
    private Vector2 stockageDirection;
    private Vector2 newDirection1;
    private Vector2 newDirection2;
    private bool directionFound1;
    private bool directionFound2;
    private int iteration;


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
            newDirection1 = direction;
            newDirection2 = direction;
            directionFound1 = false;
            directionFound2 = false;
            
            for (int i = 0; i < 8; i++)
            {
                if (!directionFound1 && !directionFound2)
                {
                    if (i % 2 == 0)
                    {
                        newDirection1 = TryNewDirection(newDirection1, true);
                    
                        if (!VerifyFall(newDirection1))
                        {
                            directionFound1 = true;
                            iteration = i;
                        }
                    }

                    else
                    {
                        newDirection2 = TryNewDirection(newDirection2, false);
                    
                        if (!VerifyFall(newDirection2))
                        {
                            directionFound2 = true;
                            iteration = i;
                        }
                    }
                }
            }

            if (directionFound1)
            {
                Vector3 desiredVelocity = new Vector3(newDirection1.x, 0f, newDirection1.y) * maxSpeed;
            
                Vector3 newResistance = ReferenceManager.Instance.cameraRotationReference.transform.InverseTransformDirection(resistanceCable);
                desiredVelocity += new Vector3(newResistance.x, 0, newResistance.z) * maxSpeed;
        
                float maxSpeedChange = maxAcceleration * Time.deltaTime;

                // Acceleration du personnage
                velocity = ReferenceManager.Instance.cameraRotationReference.transform.InverseTransformDirection(manager.rb.velocity);
                velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
                velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);
                manager.rb.velocity = ReferenceManager.Instance.cameraRotationReference.transform.TransformDirection(velocity);
            }
            
            else if (directionFound2)
            {
                Vector3 desiredVelocity = new Vector3(newDirection2.x, 0f, newDirection2.y) * maxSpeed;
            
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
    public void ClimbObject(List<GameObject> climbedObject)
    {
        GameObject currentClimbedObject = climbedObject[0];

        for (int i = 0; i < climbedObject.Count; i++)
        {
            if (climbedObject[i].transform.position.y > currentClimbedObject.transform.position.y && climbedObject[i].transform.position.y < transform.position.y + 1)
            {
                currentClimbedObject = climbedObject[i];
            }
        }
        
        if (currentClimbedObject.GetComponent<ObjetInteractible>().isClimbable)
        {
            float distance = Vector3.Distance(currentClimbedObject.transform.position, transform.position);
            
            if (VerifyFall(stockageDirection) && (currentClimbedObject.transform.position.y < transform.position.y - 0.5f || distance > 2))
            {
                transform.position = CalculateFallPos(stockageDirection);
            }
            else
            {
                transform.position = currentClimbedObject.transform.position + Vector3.up * 2;
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
        Vector3 point1 = new Vector3(0, -5000, 0);
        Vector3 point2 = new Vector3(0, -5000, 0);
        Vector3 point3 = new Vector3(0, -5000, 0);


        // Raycast 1
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit raycastHit;
        
        if(Physics.Raycast(ray, out raycastHit, 10, layerFall))
        {
            point1 = raycastHit.point;
        }
        
        
        // Raycast 2
        Vector3 newDirection = ReferenceManager.Instance.cameraRotationReference.transform.TransformDirection(new Vector3(direction.x, 0, direction.y));
        
        ray = new Ray(transform.position + (newDirection.normalized * 1f), Vector3.down);
        RaycastHit raycastHit3;

        if(Physics.Raycast(ray, out raycastHit3, 10, layerFall))
        {
            point3 = raycastHit3.point;
        }


        // Raycast3 (pentes)
        ray = new Ray(transform.position + (newDirection.normalized * 0.5f), Vector3.down);
        RaycastHit raycastHit2;

        if (Physics.Raycast(ray, out raycastHit2, 10, layerFall))
        {
            point2 = raycastHit2.point;
        }




        float difference1 = Mathf.Abs(point1.y - point2.y);
        float difference2 = Mathf.Abs(point2.y - point3.y);

        float difference3 = point1.y - point3.y;

        float difference4 = difference2 - difference1;


        if (Mathf.Abs(difference4) < 0.6f && Mathf.Abs(difference3) < 3)
        {
            return false;
        }
        else
        {
            return true;
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

    public Vector2 TryNewDirection(Vector2 currentDirection, bool negatif)
    {
        float currentAngle = Vector2.Angle(currentDirection, Vector2.up);

        float newAngle = 0;
        
        if (!negatif)
        {
            if(iteration <= 1)
                newAngle = currentAngle + 30;
            
            else
                newAngle = currentAngle + 20;
        }
        else
        {
            if(iteration <= 1)
                newAngle = currentAngle - 30;
            
            else
                newAngle = currentAngle - 20;
        }

        if (currentDirection.x < 0)
            newAngle = -newAngle;

        Vector2 newDirection = new Vector2(Mathf.Sin(Mathf.Deg2Rad * newAngle), Mathf.Cos(Mathf.Deg2Rad * newAngle));

        return newDirection;
    }



    public Vector3 CalculateFallPos(Vector2 direction)
    {
        Vector3 posFall = Vector3.zero;
        Vector3 direction2 = ReferenceManager.Instance.cameraRotationReference.transform.TransformDirection(new Vector3(direction.x, 0, direction.y));
        
        Ray ray = new Ray(transform.position + direction2 * 1.5f, Vector3.down);
        RaycastHit raycastHit2;

        if(Physics.Raycast(ray, out raycastHit2, 10))
        {
            posFall = raycastHit2.point + Vector3.up;
        }
        
        return posFall;
    }
    
}
