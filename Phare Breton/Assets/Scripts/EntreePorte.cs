using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntreePorte : MonoBehaviour
{
    [Range(1, 2)] public int numeroEntree;
    public bool goInside;    // Pour si la porte menne à l'intérieur d'un batiment

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Interactible"))
        {
            // CHANGEMENTS POSITION
            if (numeroEntree == 1)
                GetComponentInParent<Porte>().EnterDoor1(collision.gameObject);

            else
                GetComponentInParent<Porte>().EnterDoor2(collision.gameObject);


            // CHANGEMENTS CAMERA
            if (goInside)
                GetComponentInParent<Porte>().GoInside();

            else
                GetComponentInParent<Porte>().GoOutside();
        }
    }
}
