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
    [HideInInspector] public CapsuleCollider colliderChara;

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

    [Header("Son")]
    public float vitessePas;
    private float timerPas;
    public bool isOnGravier;
    public bool isOnWater;
    public bool hasEcho;


    private void Awake()
    {
        manager = GetComponent<CharaManager>();
        colliderChara = GetComponent<CapsuleCollider>();

        fallDir = Vector3.down * 550;
    }


    //------------------------------------------------------------------------------------------------------------------
    

    // BOUGE LE PERSONNAGE
    public void MoveCharacter(Vector2 direction)
    {
        if (direction != Vector2.zero && direction.magnitude > 0.9f)
        {
            stockageDirection = direction;

            PlaySoundPas();
        }

        /*if (VFXPasActif)
        {
            if(!VFXPas.isPlaying)
                VFXPas.Play();
            var main = VFXPas.main;
            //main.Start = mesh.rotation;
        }*/
        
        // Gravité
        if(!manager.isCrossingDoor)
            manager.rb.AddForce(fallDir * Time.fixedDeltaTime, ForceMode.Force);


        bool willFall = VerifyFall(direction.normalized, true);

        if (!willFall)
        {
            Vector3 desiredVelocity = new Vector3(direction.x, 0f, direction.y) * maxSpeed;
            
            Vector3 newResistance = ReferenceManager.Instance.cameraRotationReference.transform.InverseTransformDirection(resistanceCable);
            desiredVelocity += new Vector3(newResistance.x, 0, newResistance.z) * maxSpeed;
        
            float maxSpeedChange = maxAcceleration * Time.fixedDeltaTime;
            
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

        else
        {
            iteration = 0;
            
            newDirection1 = direction.normalized;
            newDirection2 = direction.normalized;
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
                    newDirection1 = TryNewDirection(newDirection1, true, 32);
                    newDirection1 *= direction.magnitude;

                    desiredVelocity = new Vector3(newDirection1.x, 0f, newDirection1.y) * (maxSpeed * ratio);
                }

                else
                {
                    newDirection2 = TryNewDirection(newDirection2, false, 32);
                    newDirection2 *= direction.magnitude;
                    
                    desiredVelocity = new Vector3(newDirection2.x, 0f, newDirection2.y) * (maxSpeed * ratio); 
                }

                Vector3 newResistance = ReferenceManager.Instance.cameraRotationReference.transform.InverseTransformDirection(resistanceCable);
                desiredVelocity += new Vector3(newResistance.x, 0, newResistance.z) * maxSpeed;
        
                float maxSpeedChange = maxAcceleration * Time.fixedDeltaTime;

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


    public void PlaySoundPas()
    {
        timerPas -= Time.deltaTime;
        
        if(timerPas < 0)
        {
            timerPas = vitessePas;

            if (!isOnGravier && !isOnWater && !hasEcho)
            {
                int index = UnityEngine.Random.Range(0, 3);

                if(index == 0)
                    AudioManager.instance.PlaySoundOneShot(0, 0, 0, manager.playerAudioSource);

                else if(index == 1)
                    AudioManager.instance.PlaySoundOneShot(1, 0, 0, manager.playerAudioSource);

                else
                    AudioManager.instance.PlaySoundOneShot(2, 0, 0, manager.playerAudioSource);
            }

            else if (hasEcho)
            {
                int index = UnityEngine.Random.Range(0, 3);

                if (index == 0)
                    AudioManager.instance.PlaySoundOneShot(5, 0, 0, manager.playerAudioSource);

                else if (index == 1)
                    AudioManager.instance.PlaySoundOneShot(6, 0, 0, manager.playerAudioSource);

                else
                    AudioManager.instance.PlaySoundOneShot(7, 0, 0, manager.playerAudioSource);
            }

            else if (isOnGravier)
            {
                int index = UnityEngine.Random.Range(0, 2);

                if (index == 0)
                    AudioManager.instance.PlaySoundOneShot(3, 0, 0, manager.playerAudioSource);

                else 
                    AudioManager.instance.PlaySoundOneShot(4, 0, 0, manager.playerAudioSource);
            }

            else if (isOnWater)
            {
                int index = UnityEngine.Random.Range(0, 2);

                if (index == 0)
                    AudioManager.instance.PlaySoundOneShot(8, 0, 0, manager.playerAudioSource);

                else
                    AudioManager.instance.PlaySoundOneShot(9, 0, 0, manager.playerAudioSource);
            }
        }
    }

    public IEnumerator MoveCharacterDoor(Vector3 newPos, float duration)
    {
        manager.isCrossingDoor = true;

        Vector3 direction = newPos - transform.position;
        RotateCharacter(new Vector2(direction.x, direction.z), true, true);

        transform.DOMove(newPos, duration);
        
        yield return new WaitForSeconds(duration);
        
        manager.isCrossingDoor = false;
    }


    public IEnumerator ClimbLadder(Vector3 finalDestination, Vector3 origin, bool goUp)
    {
        if (manager.nearBoxesDown.Count > 1 && VerifyFall(stockageDirection, true))
        {
            List<GameObject> objectToClimb = new List<GameObject>();

            for (int i = 0; i < manager.nearBoxesDown.Count; i++)
            {
                Boite currentBoite;

                if (manager.nearBoxesDown[i].TryGetComponent(out currentBoite))
                {
                    objectToClimb.Add(manager.nearBoxesDown[i]);
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
                
                colliderChara.enabled = false;

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
            colliderChara.enabled = true;
        }
    }

    public IEnumerator RotateCharaLadder(Vector3 direction, bool goUp)
    {
        float timer = 0.4f;

        while(timer > 0)
        {
            timer -= Time.deltaTime;

            if(goUp)
                RotateCharacter(new Vector2(direction.x, direction.z), true, false);

            else
                RotateCharacter(new Vector2(-direction.x, -direction.z), true, false);

            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
        

    // ORIENTATION LES CONTROLES DU PERSONNAGE EN FONCTION DE L'ANGLE DE CAMERA
    public void RotateCharacterCamera()
    {
        ReferenceManager.Instance.cameraReference.ActualiseRotationCamRef();
    }

    // ORIENTE LE MESH DU PERSONNAGE
    public void RotateCharacter(Vector2 direction, bool isLadder, bool instant)
    {
        if (direction != Vector2.zero && direction.magnitude > 0.03f)
        {
            Vector3 newDirection = ReferenceManager.Instance.cameraRotationReference.transform.TransformDirection(new Vector3(direction.x, 0, direction.y));

            if (isLadder)
                newDirection = new Vector3(direction.x, 0, direction.y);

            if (!instant)
                currentRotation = Vector3.Lerp(currentRotation, newDirection, Time.fixedDeltaTime * 17);

            else
                currentRotation = newDirection;

            mesh.rotation = Quaternion.LookRotation(currentRotation, Vector3.up) * Quaternion.Euler(0, 180, 0);
        }
    }


    // BOUGE LES OBJETS CONTROLES PAS LE JOUEUR
    public void MoveObjects(List<Rigidbody> objects, List<ObjetInteractible> scripts, Vector2 direction)
    {
        for (int k = 0; k < objects.Count; k++)
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
                
                objects[k].transform.position = new Vector3(objects[k].transform.position.x, Mathf.Lerp(objects[k].transform.position.y, scripts[k].currentHauteur, Time.fixedDeltaTime * 2), 
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

            float maxSpeedChange = maxAccelerationObject * Time.fixedDeltaTime;

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
                AudioManager.instance.PlaySoundOneShot(2, 1, 0, manager.playerAudioSource);

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
        float hauteurStop = 1.5f;
        
        Vector3 newDirection = ReferenceManager.Instance.cameraRotationReference.transform.TransformDirection(new Vector3(direction.x, 0, direction.y));
        
        Vector3 point1 = DoRaycast(transform.position, 10);
        Vector3 point2 = DoRaycast(transform.position + (newDirection.normalized * 0.1f), 10);
        Vector3 point3 = DoRaycast(transform.position + (newDirection.normalized * 0.2f), 10);
        Vector3 point4 = DoRaycast(transform.position + (newDirection.normalized * 0.3f), 10);
        Vector3 point5 = DoRaycast(transform.position + (newDirection.normalized * 0.4f), 10);
        Vector3 point6 = DoRaycast(transform.position + (newDirection.normalized * 0.5f), 10);
        Vector3 point7 = DoRaycast(transform.position + (newDirection.normalized * 0.6f), 10);
        Debug.DrawLine(point3, point2);
        Debug.DrawLine(point1, point2);
        Debug.DrawLine(point3, point4);
        Debug.DrawLine(point4, point5);


        if (enFace && Mathf.Abs(point1.y - transform.position.y) > 2)
        {
            float decalage = 0.35f;
            
            Vector3 pointR1 = DoRaycast(transform.position + mesh.transform.right * decalage, 10);
            Vector3 pointR2 = DoRaycast(transform.position + mesh.transform.right * decalage + (newDirection.normalized * 0.35f), 10);
            Vector3 pointR3 = DoRaycast(transform.position + mesh.transform.right * decalage + (newDirection.normalized * 0.7f), 10);
            Debug.DrawLine(pointR3, pointR2);
            Debug.DrawLine(pointR1, pointR2);
        
            Vector3 pointL1 = DoRaycast(transform.position - mesh.transform.right * decalage, 10);
            Vector3 pointL2 = DoRaycast(transform.position - mesh.transform.right * decalage + (newDirection.normalized * 0.35f), 10);
            Vector3 pointL3 = DoRaycast(transform.position - mesh.transform.right * decalage + (newDirection.normalized * 0.7f), 10);
            Debug.DrawLine(pointL3, pointL2);
            Debug.DrawLine(pointL1, pointL2);

            float differenceR1 = Mathf.Abs(pointR1.y - pointR2.y);
            float differenceR2 = Mathf.Abs(pointR2.y - pointR3.y);

            float differenceR3 = pointR1.y - pointR3.y;
            float differenceR4 = differenceR2 - differenceR1;
            
            
        
            float differenceL1 = Mathf.Abs(pointL1.y - pointL2.y);
            float differenceL2 = Mathf.Abs(pointL2.y - pointL3.y);

            float differenceL3 = pointL1.y - pointL3.y;
            float differenceL4 = differenceL2 - differenceL1;


            if (Mathf.Abs(differenceR4) > 0.8f || Mathf.Abs(differenceR3) > hauteurStop || differenceR1 > hauteurStop)
            {
                return true;
            }
            if (Mathf.Abs(differenceL4) > 0.8f || Mathf.Abs(differenceL3) > hauteurStop || differenceL1 > hauteurStop)
            {
                return true;
            }
        }
        
        float difference1 = Mathf.Abs(point1.y - point2.y);
        float difference2 = Mathf.Abs(point2.y - point3.y);
        float difference3 = Mathf.Abs(point3.y - point4.y);
        float difference4 = Mathf.Abs(point4.y - point5.y);
        float difference5 = Mathf.Abs(point5.y - point6.y);
        float difference6 = Mathf.Abs(point6.y - point7.y);

        float difference7 = point1.y - point7.y;
        float difference8 = difference2 - difference1 - difference3 - difference4;
        

        if (Mathf.Abs(difference8) < 0.8f && Mathf.Abs(difference7) < hauteurStop && difference1 < hauteurStop && difference4 < hauteurStop && difference3 < hauteurStop 
            && difference2 < hauteurStop && difference5 < hauteurStop && difference6 < hauteurStop)
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
