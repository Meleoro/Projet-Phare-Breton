using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntreePorte : MonoBehaviour
{
    [Range(1, 2)] public int numeroEntree;
    public bool staticCamera;
    
    [Header("Gestion des alpha")]
    public List<TransparencyObject> desactivatedObjects = new List<TransparencyObject>();
    public bool activateAuto;
    public float distanceMin = 1;     // Distance à partir de laquelle on va calculer un alpha différent de 0
    public float distanceMax = 10;     // Distance après laquelle les éléments ne sont plus affectés par des modifiations d'alpha

    // Variables permettant de détruire câble si on sort puis entre
    [HideInInspector] public bool hasCableThrough;
    [HideInInspector] public CableCreator cableThisSide;
    [HideInInspector] public CableCreator cableOtherSide;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Interactible"))
        {
            // Changements camera

            GetComponentInParent<Porte>().UseDoor(numeroEntree, collision.gameObject, staticCamera);
        }
    }
}


[Serializable]
public class TransparencyObject
{
    public MeshRenderer meshRenderer;
    
    [Header("GestionAlphaManuelle")]
    [Range(0f, 1f)] public float alpha;
}
