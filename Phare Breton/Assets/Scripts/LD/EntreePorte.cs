using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntreePorte : MonoBehaviour
{
    [Range(1, 2)] public int numeroEntree;
    public bool goInside;
    public List<MeshRenderer> desactivatedObjects = new List<MeshRenderer>();

    // Variables permettant de détruire câble si on sort puis entre
    [HideInInspector] public bool hasCableThrough;
    [HideInInspector] public CableCreator cableThisSide;
    [HideInInspector] public CableCreator cableOtherSide;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Interactible"))
        {
            // Changements positions
            if (numeroEntree == 1)
                GetComponentInParent<Porte>().EnterDoor(collision.gameObject, 1);

            else
                GetComponentInParent<Porte>().EnterDoor(collision.gameObject, 2);


            // Changements camera
            if (goInside)
                GetComponentInParent<Porte>().GoInside();

            else
                GetComponentInParent<Porte>().GoOutside();
        }
    }
}
