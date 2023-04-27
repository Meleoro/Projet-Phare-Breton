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
        float angle = 0;
        pointsGround.Clear();

        if (!inverse)
        {
            for (int i = 0; i < 8; i++)
            {
                angle += 45;
                
                Vector2 direction = new Vector2(Mathf.Sin(Mathf.Deg2Rad * angle), Mathf.Cos(Mathf.Deg2Rad * angle)) * 3;
                Vector3 posRaycast;
                
                if(posChara.position.y > center.position.y)
                    posRaycast = TPPosBas.position + new Vector3(direction.x, 0.5f, direction.y);
                
                else
                    posRaycast = TPPosHaut.position + new Vector3(direction.x, 0.5f, direction.y);
                
                Ray ray = new Ray(posRaycast, Vector3.down);
                RaycastHit raycastHit;

                if (Physics.Raycast(ray, out raycastHit, 3, LayerMask.NameToLayer("Player")))
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
                
                Vector2 direction = new Vector2(Mathf.Sin(Mathf.Deg2Rad * angle), Mathf.Cos(Mathf.Deg2Rad * angle)) * 3;
                Vector3 posRaycast;
                
                if(posChara.position.y < center.position.y)
                    posRaycast = TPPosBas.position + new Vector3(direction.x, 0.5f, direction.y);
                
                else
                    posRaycast = TPPosHaut.position + new Vector3(direction.x, 0.5f, direction.y);

                Ray ray = new Ray(posRaycast, Vector3.down);
                RaycastHit raycastHit;

                if (Physics.Raycast(ray, out raycastHit, 3, LayerMask.NameToLayer("Player")))
                {
                    if (raycastHit.collider.gameObject != gameObject)
                    {
                        pointsGround.Add(raycastHit.point);
                    }
                }
            }
        }
        
        Vector3 finalPos = Vector3.zero;
        
        for (int i = 0; i < pointsGround.Count; i++)
        {
            finalPos += pointsGround[i];
        }

        finalPos /= pointsGround.Count;

        return finalPos + new Vector3(0, 1, 0);
    }


    public void TakeLadder(Transform chara)
    {
        //chara.position = FindTPPos(chara, false);

        StartCoroutine(ReferenceManager.Instance.characterReference.movementScript.ClimbLadder(FindTPPos(chara, false),
            FindTPPos(chara, true)));
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
