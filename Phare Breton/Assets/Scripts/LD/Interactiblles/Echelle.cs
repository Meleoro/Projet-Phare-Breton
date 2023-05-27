using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Echelle : ObjetInteractible
{
    [Header("Ladder")]
    [HideInInspector] public Transform TPPosBas;
    [HideInInspector] public Transform center;
    [HideInInspector] public Transform TPPosHaut;
    [SerializeField] private bool hautNeedCollider;
    [SerializeField] private bool basNeedCollider;
    [SerializeField] private LayerMask layerColliderEchelle;
    
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

                Ray ray = new Ray(posRaycast, Vector3.down);
                RaycastHit raycastHit;

                if (needLayer)
                {
                    if (Physics.Raycast(ray, out raycastHit, distance, layerColliderEchelle))
                    {
                        if (raycastHit.collider.gameObject != gameObject)
                        {
                            pointsGround.Add(raycastHit.point);
                        }
                    }
                }
                else
                {
                    if (Physics.Raycast(ray, out raycastHit, distance, LayerMask.NameToLayer("Player")))
                    {
                        if (raycastHit.collider.gameObject != gameObject)
                        {
                            pointsGround.Add(raycastHit.point);
                        }
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

                
                Ray ray = new Ray(posRaycast, Vector3.down);
                RaycastHit raycastHit;

                if (needLayer)
                {
                    if (Physics.Raycast(ray, out raycastHit, distance, layerColliderEchelle))
                    {
                        if (raycastHit.collider.gameObject != gameObject)
                        {
                            pointsGround.Add(raycastHit.point);
                        }
                    }
                }
                else
                {
                    if (Physics.Raycast(ray, out raycastHit, distance, LayerMask.NameToLayer("Player")))
                    {
                        if (raycastHit.collider.gameObject != gameObject)
                        {
                            pointsGround.Add(raycastHit.point);
                        }
                    }
                }
            }
        }
    }


    public void TakeLadder(Transform chara)
    {
        //chara.position = FindTPPos(chara, false);

        bool goUp = center.position.y > chara.position.y;

        StartCoroutine(ReferenceManager.Instance.characterReference.movementScript.ClimbLadder(FindTPPos(chara, false) + new Vector3(0, 0.25f, 0),
            FindTPPos(chara, true), goUp, ReferenceManager.Instance.characterReference.nearObjects));
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
