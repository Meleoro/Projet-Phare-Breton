using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ColliderZoneSombre : MonoBehaviour
{
    public GameObject VFX;
    
    private bool stop;
    private bool isLighted;

    
    
    private void DesactivateZoneSombre()
    {
        isLighted = true;
    }
    
    
    private IEnumerator RepousserChara(Vector3 posWanted, Transform chara)
    {
        if (!isLighted)
        {
            ReferenceManager.Instance.characterReference.noControl = true;
        
            //chara.DOShakePosition(0.4f, 0.4f);

            CharaManager scriptChara = ReferenceManager.Instance.characterReference;

            StartCoroutine(scriptChara.SayNo());

            yield return new WaitForSeconds(1f);

            //StartCoroutine(RotateChara(posWanted - chara.position));

            yield return new WaitForSeconds(0.4f);
        
            chara.DOMove(posWanted, 0.8f);
            scriptChara.isCrossingDoor = true;
            scriptChara.isWalking = true;

            yield return new WaitForSeconds(1);

            scriptChara.isCrossingDoor = false;
            ReferenceManager.Instance.characterReference.noControl = false;
            stop = false;
        }
    }

   /* private IEnumerator RotateChara(Vector3 wantedDirection)
    {
        float timer = 1f;

        Vector3 currentDirection = -wantedDirection;
        Vector3 startDirection = -wantedDirection;
        
        

        while (timer > 0)
        {
            timer -= Time.deltaTime * 2;

            currentDirection = Vector3.Lerp(wantedDirection, startDirection, timer);

            ReferenceManager.Instance.characterReference.movementScript.RotateCharacter(new Vector2(currentDirection.x, currentDirection.z).normalized, true, false);
            
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }*/
    

    private void OnTriggerStay(Collider other)
    {
        if (!isLighted)
        {
            if (other.CompareTag("Player"))
            {
                if (!ReferenceManager.Instance.characterReference.noControl &&
                    !ReferenceManager.Instance.characterReference.isInLightSource && !other.isTrigger && !stop)
                {
                    stop = true;
                
                    Vector3 posWanted = other.transform.position + ReferenceManager.Instance.characterReference.movementScript.mesh.transform.forward;

                    StartCoroutine(RepousserChara(posWanted, other.transform));
                }
            }

            else if (other.CompareTag("Interactible") && !other.isTrigger)
            {
                other.GetComponent<ObjetInteractible>().isInDarkZone = true;
            }
        }

        else
        {
            if (other.CompareTag("Interactible") && !other.isTrigger)
            {
                other.GetComponent<ObjetInteractible>().isInDarkZone = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactible"))
        {
            other.GetComponent<ObjetInteractible>().isInDarkZone = false;
        }
    }
}
