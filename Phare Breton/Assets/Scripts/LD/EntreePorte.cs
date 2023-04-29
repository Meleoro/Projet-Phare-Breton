using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntreePorte : MonoBehaviour
{
    [Range(1, 2)] public int numeroEntree;
    public bool staticCamera;
    
    [Header("Gestion des alpha")]
    public List<TransparencyObject> desactivatedObjects = new List<TransparencyObject>();
    public float distanceMinCamera = 1;     // Distance à partir de laquelle on va calculer un alpha différent de 0
    public float distanceMaxCamera = 10;     // Distance après laquelle les éléments ne sont plus affectés par des modifiations d'alpha
    public float distanceMinChara = 1;     // Distance à partir de laquelle on va calculer un alpha différent de 0
    public float distanceMaxChara = 10;     // Distance après laquelle les éléments ne sont plus affectés par des modifiations d'alpha
    private bool doOnce;

    // Variables permettant de détruire câble si on sort puis entre
    [HideInInspector] public bool hasCableThrough;
    [HideInInspector] public CableCreator cableThisSide;
    [HideInInspector] public CableCreator cableOtherSide;



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Interactible"))
        {
            if (!doOnce)
                CreateListDesactivatedObjects();
            
            if (!ReferenceManager.Instance.cameraReference.scriptFondu.isInTransition)
            {
                // Changements camera
                ReferenceManager.Instance.cameraReference.isStatic = true;

                GetComponentInParent<Porte>().UseDoor(numeroEntree, collision.gameObject, staticCamera);
            }
        }
    }



    private void CreateListDesactivatedObjects()
    {
        doOnce = true;
        
        for (int i = 0; i < desactivatedObjects.Count; i++)
        {
            desactivatedObjects[i].meshRenderers =
                desactivatedObjects[i].objectsParent.GetComponentsInChildren<MeshRenderer>().ToList();
        }
    }
}


[Serializable]
public class TransparencyObject
{
    public GameObject objectsParent;
    [HideInInspector] public List<MeshRenderer> meshRenderers;

    [Header("Gestion Alpha Chara/Camera")] 
    public bool fromChara;
    
    [Header("GestionAlphaManuelle")] 
    public bool alphaManuelle;
    [Range(0f, 1f)] public float alpha;
}
