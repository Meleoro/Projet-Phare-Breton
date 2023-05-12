using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Echelle : ObjetInteractible
{
    [Header("Ladder")]
    [SerializeField] private Transform TPPosBas;
    [SerializeField] private Transform center;
    [SerializeField] private Transform TPPosHaut;
    
    private List<Vector3> pointsGround = new List<Vector3>();

    public Vector3 FindTPPos(Transform posChara, bool inverse)
    {
        pointsGround.Clear();

        Vector3 finalPos = Vector3.zero;
        
        
        RaycastPos(posChara, inverse, 3);

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
            RaycastPos(posChara, inverse, 1.5f);

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
                }

                else
                {
                    posRaycast = TPPosHaut.position + new Vector3(direction.x, 0.5f, direction.y);
                    distance = 5;
                }

                Ray ray = new Ray(posRaycast, Vector3.down);
                RaycastHit raycastHit;

                if (Physics.Raycast(ray, out raycastHit, distance, LayerMask.NameToLayer("Player")))
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
                }

                else
                {
                    posRaycast = TPPosHaut.position + new Vector3(direction.x, 0.5f, direction.y);
                    distance = 5;
                }

                Ray ray = new Ray(posRaycast, Vector3.down);
                RaycastHit raycastHit;
                
                //Debug.DrawRay(posRaycast, Vector3.down, Color.blue, 5);

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


    public void TakeLadder(Transform chara)
    {
        //chara.position = FindTPPos(chara, false);

        bool goUp = center.position.y > chara.position.y;

        StartCoroutine(ReferenceManager.Instance.characterReference.movementScript.ClimbLadder(FindTPPos(chara, false),
            FindTPPos(chara, true), goUp, ReferenceManager.Instance.characterReference.nearObjects));
    }
    
    
    private void OnTriggerEnter(Collider other)
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
    }
    
    
}
