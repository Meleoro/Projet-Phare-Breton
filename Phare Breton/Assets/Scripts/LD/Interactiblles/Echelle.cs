using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Echelle : ObjetInteractible
{
    [Header("Ladder")]
    public Transform TPPos;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ReferenceManager.Instance.characterReference.nearLadder = true;
            ReferenceManager.Instance.characterReference.ladderTPPos = TPPos.position;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ReferenceManager.Instance.characterReference.nearLadder = false;
        }
    }
}
