using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Echelle : ObjetInteractible
{
    [Header("Ladder")]
    [SerializeField]
    private BoxCollider _collider;
    [SerializeField] private bool hautNeedCollider;
    [SerializeField] private bool basNeedCollider;
    [SerializeField] private LayerMask layerColliderEchelle;
    [HideInInspector] public Transform TPPosBas;
    [HideInInspector] public Transform center;
    [HideInInspector] public Transform TPPosHaut;

    private RaycastHit raycastHit;
    
    private List<Vector3> pointsGround = new List<Vector3>();


    public bool VerifyUse(Transform posChara)
    {
        pointsGround.Clear();
        
        RaycastPos(posChara, false, 1.2f);

        if (pointsGround.Count == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    
    
    public Vector3 FindTPPos(Transform posChara, bool inverse)
    {
        pointsGround.Clear();

        Vector3 finalPos = Vector3.zero;
        
        
        RaycastPos(posChara, inverse, 1.2f);

        if (pointsGround.Count != 0)
        {
            for (int i = 0; i < pointsGround.Count; i++)
            {
                finalPos += pointsGround[i];
            }
            
            finalPos /= pointsGround.Count;


            if (inverse)
            {
                Vector2 direction = new Vector2(transform.position.x - finalPos.x, transform.position.z - finalPos.z);

                if (direction.magnitude > 0.4f)
                {
                    finalPos = new Vector3(transform.position.x, finalPos.y, transform.position.z) - new Vector3(direction.normalized.x * 0.4f, 0, direction.normalized.y * 0.4f);
                }
            }
        }
        
        else
        {
            RaycastPos(posChara, inverse, 0.75f);

            if (pointsGround.Count != 0)
            {
                for (int i = 0; i < pointsGround.Count; i++)
                {
                    finalPos += pointsGround[i];
                }
                
                finalPos /= pointsGround.Count;
            }
            
            else
            {
                if (posChara.position.y < center.position.y)
                {
                    return ReferenceManager.Instance.characterReference.transform.position;
                }

                else
                {
                    return TPPosBas.position;
                }
            }
        }
        
        return finalPos + new Vector3(0, 1, 0);
    }


    public void RaycastPos(Transform posChara, bool inverse, float rayon)
    {
        float angle = 0;
        bool needLayer = false;
        
        if (!inverse)
        {
            for (int i = 0; i < 8; i++)
            {
                angle += 45;
                
                Vector2 direction = new Vector2(Mathf.Sin(Mathf.Deg2Rad * angle), Mathf.Cos(Mathf.Deg2Rad * angle)) * rayon;
                Vector3 posRaycast;
                float distance;

                if (posChara.position.y > center.position.y)
                {
                    posRaycast = TPPosBas.position + new Vector3(direction.x, 0.5f, direction.y);
                    distance = 2;
                    
                    if(basNeedCollider)
                        needLayer = true;
                }

                else
                {
                    posRaycast = TPPosHaut.position + new Vector3(direction.x, 0.5f, direction.y);
                    distance = 5;
                    
                    if(hautNeedCollider)
                        needLayer = true;
                }

                if (CreateRaycast(posRaycast, distance, needLayer))
                {
                    if (raycastHit.collider.gameObject != gameObject)
                    {
                        pointsGround.Add(raycastHit.point); 
                        
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < 8; i++)
            {
                angle += 45;
                
                Vector2 direction = new Vector2(Mathf.Sin(Mathf.Deg2Rad * angle), Mathf.Cos(Mathf.Deg2Rad * angle)) * rayon;
                Vector3 posRaycast;
                float distance;

                if (posChara.position.y < center.position.y)
                {
                    posRaycast = TPPosBas.position + new Vector3(direction.x, 0.5f, direction.y);
                    distance = 2;
                    
                    if(basNeedCollider)
                        needLayer = true;
                }

                else
                {
                    posRaycast = TPPosHaut.position + new Vector3(direction.x, 0.5f, direction.y);
                    distance = 5;

                    if(hautNeedCollider)
                        needLayer = true;
                }
                

                if (CreateRaycast(posRaycast, distance, needLayer))
                {
                    if (raycastHit.collider.gameObject != gameObject)
                    {
                        pointsGround.Add(raycastHit.point); 
                        
                    }
                }
               
            }
        }
    }


    public bool CreateRaycast(Vector3 startPos, float lenght, bool needLayer)
    {
        Ray ray = new Ray(startPos, Vector3.down);

        if (lenght < 0)
        {
            return false;
        }

        if (needLayer)
        {
            if (Physics.Raycast(ray, out raycastHit, lenght, layerColliderEchelle))
            {
                //Debug.DrawLine(startPos, raycastHit.point);
                
                if (raycastHit.collider.gameObject != gameObject && !raycastHit.collider.isTrigger)
                {
                    return true;
                }

                else
                {
                    return DoRaycast(raycastHit.point - Vector3.down * 0.001f, lenght - raycastHit.distance);
                }
            }

            else
            {
                return false;
            }
        }

        else
        {
            if (Physics.Raycast(ray, out raycastHit, lenght, LayerMask.NameToLayer("Player")))
            {
                //Debug.DrawLine(startPos, raycastHit.point);

                if (raycastHit.collider.gameObject != gameObject && !raycastHit.collider.isTrigger)
                {
                    return true;
                }

                else
                {
                    return DoRaycast(raycastHit.point, lenght - raycastHit.distance);
                }
            }

            else
            {
                //Debug.DrawLine(startPos, startPos + Vector3.down * lenght);
                
                return false;
            }
        }
    }


    public void TakeLadder(Transform chara)
    {
        //chara.position = FindTPPos(chara, false);

        bool goUp = center.position.y > chara.position.y;

        Vector3 finalDestination = FindTPPos(chara, false) + new Vector3(0, 0.2f, 0);
        Vector3 startPos = FindTPPos(chara, true);

        Vector3 direction = finalDestination - startPos;
        Vector2 newDirection = new Vector2(direction.x, direction.z).normalized * 0.45f;

        finalDestination = new Vector3(center.position.x + newDirection.x, finalDestination.y, center.position.z + newDirection.y);
        startPos = new Vector3(center.position.x - newDirection.x, startPos.y, center.position.z - newDirection.y);

        Debug.DrawLine(startPos, finalDestination, Color.red, 1);

        StartCoroutine(ReferenceManager.Instance.characterReference.movementScript.ClimbLadder(finalDestination,
            startPos, goUp, ReferenceManager.Instance.characterReference.nearObjects, _collider));
    }
    
    
    /*private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ReferenceManager.Instance.characterReference.nearLadder = this;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ReferenceManager.Instance.characterReference.nearLadder = null;
        }
    }*/
    
    
}
