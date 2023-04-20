using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ColliderZoneSombre : MonoBehaviour
{
    private IEnumerator RepousserChara(Vector3 posWanted, Transform chara)
    {
        ReferenceManager.Instance.characterReference.noControl = true;
        
        chara.DOShakePosition(0.4f, 0.4f);

        yield return new WaitForSeconds(0.4f);

        chara.DOMove(posWanted, 1);

        yield return new WaitForSeconds(1);

        ReferenceManager.Instance.characterReference.noControl = false;
    }
    

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!ReferenceManager.Instance.characterReference.noControl &&
                !ReferenceManager.Instance.characterReference.isInLightSource && !other.isTrigger)
            {
                Vector3 posWanted = other.transform.position + other.transform.right;

                StartCoroutine(RepousserChara(posWanted, other.transform));
            }
        }

        else if (other.CompareTag("Interactible") && !other.isTrigger)
        {
            other.GetComponent<ObjetInteractible>().isInDarkZone = true;
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
