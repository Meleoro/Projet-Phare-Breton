using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntreePorte : MonoBehaviour
{
    [Range(1, 2)] public int numeroEntree;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(numeroEntree == 1)
            {
                GetComponentInParent<Porte>().EnterDoor1();
            }

            else
            {
                GetComponentInParent<Porte>().EnterDoor2();
            }
        }
    }
}
