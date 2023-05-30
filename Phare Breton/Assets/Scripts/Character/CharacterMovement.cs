using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    [Header("References")] [SerializeField]
    public Transform mesh;
    private CharaManager manager;
    public ParticleSystem VFXPas;

    [Header("Movements")]
    [SerializeField, Range(0f, 100f)] float maxSpeed = 10f;
    [SerializeField, Range(0f, 100f)] float maxAcceleration = 10f;
    private Vector3 velocity;
    [HideInInspector] public bool doOnce;
    [HideInInspector] public Vector3 resistanceCable;
    private Vector3 currentRotation;
    [HideInInspector] public bool VFXPasActif;

    [Header("MovementsObjets")]
    public float hauteurObject = 0.5f;
    [SerializeField, Range(0f, 100f)] float maxSpeedObject = 10f;
    [SerializeField, Range(0f, 100f)] float maxAccelerationObject = 10f;
    private Vector3 velocityObject;
    [HideInInspector] public Vector3 resistanceCableObject1;
    [HideInInspector] public Vector3 resistanceCableObject2;

    [Header("Fall")] 
    [SerializeField] private LayerMask layerFall;
    [HideInInspector] public Vector2 stockageDirection;
    private Vector2 newDirection1;
    private Vector2 newDirection2;
    private bool directionFound1;
    private bool directionFound2;
    private int iteration;
    private Vector3 fallDir;
    public bool willFall;
    private RaycastHit fallRaycastHit;


    private void Awake()
    {
        manager = GetComponent<CharaManager>();

        fallDir = Vector3.down * 250;
    }


    //------------------------------------------------------------------------------------------------------------------
    

    // BOUGE LE PERSONNAGE
    public void MoveCharacter(Vector2 direction)
    {
        if (direction != Vector2.zero && direction.magnitude > 0.9f)
        {
            stockageDirection = direction;
        }

        /*if (VFXPasActif)
        {
            if(!VFXPas.isPlaying)
                VFXPas.Play();
            var main = VFXPas.main;
            //main.Start = mesh.rotation;
        }*/
        
        // Gravité
        manager.rb.AddForce(fallDir * Time.deltaTime, ForceMode.Force);


        bool willFall = VerifyFall(direction, true);

        if (!willFall)
        {
            Vector3 desiredVelocity = new Vector3(direction.x, 0f, direction.y) * maxSpeed;
            
            Vector3 newResistance = ReferenceManager.Instance.cameraRotationReference.transform.InverseTransformDirection(resistanceCable);
            desiredVelocity += new Vector3(newResistance.x, 0, newResistance.z) * maxSpeed;
        
            float maxSpeedChange = maxAcceleration * Time.deltaTime;
            
            if (direction == Vector2.zero)
            {
                maxSpeedChange *= 2;
            }

            // Acceleration du personnage
            velocity = ReferenceManager.Instance.cameraRotationReference.transform.InverseTransformDirection(manager.rb.velocity);
            velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
            velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);
            manager.rb.velocity = ReferenceManager.Instance.cameraRotationReference.transform.TransformDirection(velocity);
        }
        else if(direction.magnitude > 0.4f)
        {
            iteration = 0;
            
            newDirection1 = direction;
            newDirection2 = direction;
            directionFound1 = false;
            directionFound2 = false;
            
            for (int i = 0; i < 13; i++)
            {
                if (!directionFound1 && !directionFound2)
                {
                    iteration = i + 1;
                    
                    if (i % 2 == 0)
                    {
                        newDirection1 = TryNewDirection(newDirection1, true, 0);
                    
                        if (!VerifyFall(newDirection1, false))
                        {
                            directionFound1 = true;
                        }
                    }

                    else
                    {
                        newDirection2 = TryNewDirection(newDirection2, false, 0);
                    
                        if (!VerifyFall(newDirection2, false))
                        {
                            directionFound2 = true;
                        }
                    }
                }
            }

            if (directionFound1 || directionFound2)
            {
                Vector3 desiredVelocity;
                float ratio = 1 - (float)iteration / 13 ;

                if (directionFound1)
                {
                    newDirection1 = TryNewDirection(newDirection1, true, 30);
                    newDirection1 *= direction.magnitude;

                    desiredVelocity = new Vector3(newDirection1.x, 0f, newDirection1.y) * (maxSpeed * ratio);
                }

                else
                {
                    newDirection2 = TryNewDirection(newDirection2, false, 30);
                    newDirection2 *= direction.magnitude;
                    
                    desiredVelocity = new Vector3(newDirection2.x, 0f, newDirection2.y) * (maxSpeed * ratio); 
                }

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


    public IEnumerator ClimbLadder(Vector3 finalDestination, Vector3 origin, bool goUp, List<GameObject> nearObjects, BoxCollider echelleCollider)
    {
        if (nearObjects.Count > 1 && VerifyFall(stockageDirection, true))
        {
            List<GameObject> objectToClimb = new List<GameObject>();

            for (int i = 0; i < nearObjects.Count; i++)
            {
                Boite currentBoite;

                if (nearObjects[i].TryGetComponent(out currentBoite))
                {
                    objectToClimb.Add(nearObjects[i]);
                }
            }

            ClimbObject(objectToClimb);
        }

        else
        {
            //echelleCollider.enabled = false;
            manager.rb.isKinematic = true;

            manager.anim.SetBool("isGoingUp", goUp);


            float hauteur = Mathf.Abs(origin.y - finalDestination.y);
            Vector3 direction = (finalDestination - origin).normalized;

            StartCoroutine(RotateCharaLadder(direction, goUp));

            manager.noControl = true;

            transform.DOMove(origin, 0.4f).SetEase(Ease.OutQuad);

            if(goUp)
                manager.anim.SetTrigger("startLadder");

            else
                manager.anim.SetTrigger("endLadder");


            yield return new WaitForSeconds(0.4f);

            if (goUp)
            {
                float duration1 = hauteur * 0.3f;
                float duration3 = 0.35f;

                transform.DOMoveY(finalDestination.y - 1, duration1).SetEase(Ease.Linear);

                yield return new WaitForSeconds(duration1 - 0.36f);

                manager.anim.SetTrigger("endLadder");

                yield return new WaitForSeconds(0.36f);

                transform.DOMoveY(finalDestination.y - 1f, 0.1f).SetEase(Ease.Linear);

                yield return new WaitForSeconds(0.1f);

                transform.DOMoveY(finalDestination.y, duration3 + 0.3f).SetEase(Ease.Linear);

                transform.DOMoveX(finalDestination.x, duration3).SetEase(Ease.Linear);
                transform.DOMoveZ(finalDestination.z, duration3).SetEase(Ease.Linear);

                yield return new WaitForSeconds(duration3 + 0.3f);
            }

            else
            {
                transform.DOMove(new Vector3(finalDestination.x, transform.position.y, finalDestination.z), 0.3f).SetEase(Ease.Linear);

                //yield return new WaitForSeconds(0.5f);

                transform.DOMoveY(finalDestination.y, hauteur * 0.4f).SetEase(Ease.Linear);;

                yield return new WaitForSeconds(hauteur * 0.4f - 0.5f);

                manager.anim.SetTrigger("startLadder");

                yield return new WaitForSeconds(0.5f);
            }

            manager.rb.isKinematic = false;
            //echelleCollider.enabled = true;
            manager.noControl = false;
        }
    }

    IEnumerator RotateCharaLadder(Vector3 direction, bool goUp)
    {
        float timer = 0.4f;

        while(timer > 0)
        {
            timer -= Time.deltaTime;

            if(goUp)
                RotateCharacter(new Vector2(direction.x, direction.z), true);

            else
                RotateCharacter(new Vector2(-direction.x, -direction.z), true);

            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
        

    // ORIENTATION LES CONTROLES DU PERSONNAGE EN FONCTION DE L'ANGLE DE CAMERA
    public void RotateCharacterCamera()
    {
        ReferenceManager.Instance.cameraReference.ActualiseRotationCamRef();
    }

    // ORIENTE LE MESH DU PERSONNAGE
    public void RotateCharacter(Vector2 direction, bool isLadder)
    {
        if (direction != Vector2.zero && direction.magnitude > 0.03f)
        {
            Vector3 newDirection = ReferenceManager.Instance.cameraRotationReference.transform.TransformDirection(new Vector3(direction.x, 0, direction.y));

            if (isLadder)
                newDirection = new Vector3(direction.x, 0, direction.y);
            
            currentRotation = Vector3.Lerp(currentRotation, newDirection, Time.deltaTime * 17);

            mesh.rotation = Quaternion.LookRotation(currentRotation, Vector3.up) * Quaternion.Euler(0, 180, 0);
        }
    }


    // BOUGE LES OBJETS CONTROLES PAS LE JOUEUR
    public void MoveObjects(List<Rigidbody> objects, List<ObjetInteractible> scripts, Vector2 direction)
    {
        for(int k = 0; k < objects.Count; k++)
        {
            if (!scripts[k].isMagneted)
            {
                // Levitation de l'objet
                /*if (objects[k].transform.position.y < scripts[k].currentHauteur)
                {
                    objects[k].AddForce(Vector3.up * (1000 * Time.fixedDeltaTime), ForceMode.Acceleration);
                }
                else
                {
                    objects[k].transform.position = new Vector3(objects[k].transform.position.x, scripts[k].currentHauteur, objects[k].transform.position.z);
                }*/
                
                objects[k].transform.position = new Vector3(objects[k].transform.position.x, Mathf.Lerp(objects[k].transform.position.y, scripts[k].currentHauteur, Time.deltaTime * 2), 
                    objects[k].transform.position.z);
                scripts[k].rb.velocity = new Vector3(scripts[k].rb.velocity.x, 0, scripts[k].rb.velocity.z);
            }

            Vector3 desiredVelocity = new Vector3(direction.x, 0f, direction.y) * maxSpeedObject;

            if (manager.R2)
            {
                desiredVelocity = Vector3.zero;
            }

            if (scripts[k].isLinked)
            {
                Vector3 newResistance = ReferenceManager.Instance.cameraRotationReference.transform.InverseTransformDirection(scripts[k].resistanceCable);
                desiredVelocity += new Vector3(newResistance.x, 0, newResistance.z) * maxSpeedObject;
            }

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
            if (Vector3.Distance(climbedObject[i].transform.position, transform.position) < Vector3.Distance(currentClimbedObject.transform.position, transform.position))
            {
                currentClimbedObject = climbedObject[i];
            }
        }

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
            
            if (VerifyFall(stockageDirection, true) && (currentClimbedObject.transform.position.y < transform.position.y - 0.5f || distance > 2))
            {
                StartCoroutine(ClimbObject(CalculateFallPos(stockageDirection), transform.position, false));

                manager.anim.SetTrigger("startChute");
            }
            else if (!(currentClimbedObject.transform.position.y + 0.5f < transform.position.y))
            {
                StartCoroutine(ClimbObject(currentClimbedObject.transform.position + Vector3.up * 2, transform.position, true));

                manager.anim.SetTrigger("startBigClimb");
            }
        }
    }

    public IEnumerator ClimbObject(Vector3 finalPos, Vector3 originPos, bool goUp)
    {
        manager.noControl = true;

        Vector3 direction = originPos - finalPos;
        direction = direction.normalized;

        Vector3 startPos = finalPos + direction * 1.9f;
        Vector3 endPos = finalPos;

        StartCoroutine(RotateCharaLadder(-direction, true));

        
        yield return new WaitForSeconds(0.2f);
        
        if (goUp)
        {
            transform.DOMove(new Vector3(startPos.x, transform.position.y, startPos.z), 0.2f);
            endPos = new Vector3(finalPos.x + direction.x * 0.5f, finalPos.y, finalPos.z + direction.z * 0.5f);

            transform.DOMoveY(finalPos.y - 1.2f, 0.5f).SetEase(Ease.InOutBack);
        
            yield return new WaitForSeconds(0.5f);
        
            transform.DOMove(endPos, 0.6f).SetEase(Ease.InOutSine);

            yield return new WaitForSeconds(0.6f);
        }

        else
        {
            //transform.DOMove(new Vector3(finalPos.x, transform.position.y, finalPos.z), 0.2f);

            transform.DOMoveX(finalPos.x, 0.4f).SetEase(Ease.OutSine);
            transform.DOMoveZ(finalPos.z, 0.4f).SetEase(Ease.OutSine);

            transform.DOMoveY(finalPos.y, 0.4f).SetEase(Ease.InCubic);
        
            yield return new WaitForSeconds(0.4f);
        }
        
        manager.noControl = false;
    }

    public void GoDown()
    {
        if (VerifyFall(stockageDirection, true))
        {
            transform.position = CalculateFallPos(stockageDirection);
        }
    }


    public bool VerifyFall(Vector2 direction, bool enFace)
    {
        Vector3 newDirection = ReferenceManager.Instance.cameraRotationReference.transform.TransformDirection(new Vector3(direction.x, 0, direction.y));
        
        Vector3 point1 = DoRaycast(transform.position, 10);
        Vector3 point2 = DoRaycast(transform.position + (newDirection.normalized * 0.35f), 10);
        Vector3 point3 = DoRaycast(transform.position + (newDirection.normalized * 0.7f), 10);
        Debug.DrawLine(point3, point2);
        Debug.DrawLine(point1, point2);


        if (enFace && Mathf.Abs(point1.y - transform.position.y) > 2)
        {
            float decalage = 0.35f;
            
            Vector3 point21 = DoRaycast(transform.position - mesh.transform.right * decalage, 10);
            Vector3 point22 = DoRaycast(transform.position - mesh.transform.right * decalage + (newDirection.normalized * 0.35f), 10);
            Vector3 point23 = DoRaycast(transform.position - mesh.transform.right * decalage + (newDirection.normalized * 0.7f), 10);
            Debug.DrawLine(point23, point22);
            Debug.DrawLine(point21, point22);
        
            Vector3 point31 = DoRaycast(transform.position + mesh.transform.right * decalage, 10);
            Vector3 point32 = DoRaycast(transform.position + mesh.transform.right * decalage + (newDirection.normalized * 0.35f), 10);
            Vector3 point33 = DoRaycast(transform.position + mesh.transform.right * decalage + (newDirection.normalized * 0.7f), 10);
            Debug.DrawLine(point33, point32);
            Debug.DrawLine(point31, point32);
            
            float difference21 = 0;
            float difference22 = 0;
        
            difference21 = Mathf.Abs(point21.y - point22.y);
            difference22 = Mathf.Abs(point22.y - point33.y);

            float difference23 = point21.y - point33.y;
            float difference24 = difference22 - difference21;
            
            float difference31 = 0;
            float difference32 = 0;
        
            difference31 = Mathf.Abs(point31.y - point32.y);
            difference32 = Mathf.Abs(point32.y - point33.y);

            float difference33 = point1.y - point33.y;
            float difference34 = difference32 - difference31;


            if (Mathf.Abs(difference24) > 0.8f || Mathf.Abs(difference23) > 2.5f || difference21 > 2.5f)
            {
                return true;
            }
            if (Mathf.Abs(difference34) > 0.8f || Mathf.Abs(difference33) > 2.5f || difference31 > 2.5f)
            {
                return true;
            }
        }


        float difference1 = 0;
        float difference2 = 0;
        
        difference1 = Mathf.Abs(point1.y - point2.y);
        difference2 = Mathf.Abs(point2.y - point3.y);

        float difference3 = point1.y - point3.y;
        float difference4 = difference2 - difference1;


        if (Mathf.Abs(difference4) < 0.8f && Mathf.Abs(difference3) < 2.5f && difference1 < 2.5f)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    

    public Vector3 DoRaycast(Vector3 startPos, float lenght)
    {
        Ray ray = new Ray(startPos, Vector3.down);
        RaycastHit raycastHit;

        if(lenght <= 0)
            return new Vector3(startPos.x, -5000, startPos.z);
        
        if (Physics.Raycast(ray, out raycastHit, lenght, layerFall))
        {
            Debug.DrawLine(startPos, raycastHit.point);
            
            if (raycastHit.collider.isTrigger)
                return DoRaycast(raycastHit.point + Vector3.down * 0.01f, lenght - raycastHit.distance);
            
            if(raycastHit.collider.gameObject != gameObject)
                return raycastHit.point;
        }
        
        return new Vector3(startPos.x, -5000, startPos.z);
    }
    

    public Vector2 TryNewDirection(Vector2 currentDirection, bool negatif, float angleAdded)
    {
        float currentAngle = Vector2.Angle(currentDirection, Vector2.up);

        float newAngle = 0;
        
        if (currentDirection.x < 0)
            currentAngle = -currentAngle;
        
        if (!negatif)
        {
            if(angleAdded > 0)
                newAngle = currentAngle + angleAdded;
            
            else
                newAngle = currentAngle + 10;
        }
        else
        {
            if(angleAdded > 0)
                newAngle = currentAngle - angleAdded;
            
            else
                newAngle = currentAngle - 10;
        }

        Vector2 newDirection = new Vector2(Mathf.Sin(Mathf.Deg2Rad * newAngle), Mathf.Cos(Mathf.Deg2Rad * newAngle));

        /*Vector3 debugDir = ReferenceManager.Instance.cameraRotationReference.transform.TransformDirection(new Vector3(newDirection.x, 0, newDirection.y));
        
        if(iteration <= 2)
            Debug.DrawRay(transform.position + (debugDir.normalized * 1f), Vector3.down, Color.blue);
        
        else if(iteration <= 4)
            Debug.DrawRay(transform.position + (debugDir.normalized * 1f), Vector3.down, Color.red);*/

        return newDirection;
    }



    public Vector3 CalculateFallPos(Vector2 direction)
    {
        Vector3 posFall = Vector3.zero;
        Vector3 direction2 = ReferenceManager.Instance.cameraRotationReference.transform.TransformDirection(new Vector3(direction.x, 0, direction.y));

        List<Vector3> possibleFallPos = new List<Vector3>();

        for (int i = 0; i < 8; i++)
        {
            /*Ray ray = new Ray(transform.position + direction2 * (1f + i * 0.2f), Vector3.down);
            RaycastHit raycastHit2;

            if(Physics.Raycast(ray, out raycastHit2, 5, layerFall))
            {
                possibleFallPos.Add(raycastHit2.point + Vector3.up);
            }*/

            if (DoRaycastFallPos(transform.position + Vector3.up + direction2 * (1f + i * 0.2f), 5))
            {
                possibleFallPos.Add(fallRaycastHit.point + Vector3.up);
            }
        }

        
        List<Hauteur> hauteurs = new List<Hauteur>();

        for (int i = 0; i < possibleFallPos.Count; i++)
        {
            float currentHauteur = Mathf.Abs(possibleFallPos[i].y - transform.position.y);
            bool addHauteur = true;

            for (int k = 0; k < hauteurs.Count; k++)
            {
                if (Mathf.Abs(hauteurs[k].hauteurDiff - currentHauteur) < 0.1f)
                {
                    addHauteur = false;
                    hauteurs[k].nbrIterations += 1;
                    
                    hauteurs[k].positions.Add(possibleFallPos[i]);
                }
            }

            if (addHauteur)
            {
                hauteurs.Add(new Hauteur());

                hauteurs[hauteurs.Count - 1].hauteurDiff = currentHauteur;
                hauteurs[hauteurs.Count - 1].nbrIterations = 1;
                
                hauteurs[hauteurs.Count - 1].positions.Add(possibleFallPos[i]);
            }
        }

        int indexChose = 0;
        int currentNbrIterations = 0;
        
        for (int i = 0; i < hauteurs.Count; i++)
        {
            if (currentNbrIterations < hauteurs[i].nbrIterations)
            {
                indexChose = i;
                currentNbrIterations = hauteurs[i].nbrIterations;
            }
        }


        for (int i = 0; i < hauteurs[indexChose].positions.Count; i++)
        {
            posFall += hauteurs[indexChose].positions[i];
        }

        posFall /= hauteurs[indexChose].nbrIterations;
        
        
        /*Ray ray = new Ray(transform.position + direction2 * 1.5f, Vector3.down);
        RaycastHit raycastHit2;

        if(Physics.Raycast(ray, out raycastHit2, 10))
        {
            posFall = raycastHit2.point + Vector3.up;
        }*/
        
        return posFall;
    }
    
    public bool DoRaycastFallPos(Vector3 startPos, float lenght)
    {
        Ray ray = new Ray(startPos, Vector3.down);
        RaycastHit raycastHit;

        if(lenght <= 0)
            return false;
        
        if (Physics.Raycast(ray, out raycastHit, lenght, layerFall))
        {
            Debug.DrawLine(startPos, raycastHit.point);
            
            if (raycastHit.collider.isTrigger)
                return DoRaycastFallPos(raycastHit.point + Vector3.down * 0.01f, lenght - raycastHit.distance);

            if (raycastHit.collider.gameObject != gameObject)
            {
                fallRaycastHit = raycastHit;
                
                return true;
            }
        }
        
        return false;
    }
    
}


public class Hauteur
{
    public List<Vector3> positions = new List<Vector3>();

    public float hauteurDiff;
    public int nbrIterations;
}
