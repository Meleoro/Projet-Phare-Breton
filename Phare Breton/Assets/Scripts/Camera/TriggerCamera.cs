using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TriggerCamera : MonoBehaviour
{
    [SerializeField] private BoxCollider _collider;
    [SerializeField] private Transform cameraPos;

    [Header("Gestion des alpha")]
    [SerializeField] private List<TransparencyObject> desactivatedObjects = new List<TransparencyObject>();
    [SerializeField] private float distanceMinCamera = 1;
    [SerializeField] private float distanceMaxCamera = 8;
    [SerializeField] private float distanceMinChara = 1;
    [SerializeField] private float distanceMaxChara = 8;
    private bool doOnce;
    
    [Header("Gizmos")]
    [SerializeField] private Color mainGizmosColor;
    [SerializeField] private bool cameraView;
    [SerializeField] private Color cameraViewColor;
    [SerializeField] private float rangeCamera;
    private Camera currentCamera;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Interactible"))
        {
            // Points qui vont permettre de determiner de quel côté arrive le joueur
            Vector3 posAvant = transform.position + transform.forward;
            Vector3 posArriere = transform.position - transform.forward;

            
            if (Vector3.Distance(posArriere, other.transform.position) <
                Vector3.Distance(posAvant, other.transform.position))
            {
                ReferenceManager.Instance.cameraReference.isStatic = true;
                
                ReferenceManager.Instance.cameraReference.transform.position = cameraPos.position;
                ReferenceManager.Instance.cameraReference.transform.rotation = cameraPos.rotation;
                
                CreateListDesactivatedObjects();
                ReferenceManager.Instance.cameraReference.ActualiseDesactivatedObjects(desactivatedObjects, distanceMinCamera, distanceMaxCamera, 
                    distanceMinChara, distanceMaxChara);
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


    private void OnDrawGizmos()
    {
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.matrix = rotationMatrix;
        
        Gizmos.color = mainGizmosColor;
        Gizmos.DrawCube(_collider.center, _collider.size);
    }

    private void OnDrawGizmosSelected()
    {
        if (cameraView)
        {
            Gizmos.color = cameraViewColor;

            Matrix4x4 tempMatrix = Gizmos.matrix;
            
            if (currentCamera == null)
                currentCamera = ReferenceManager.Instance._camera;

            Gizmos.matrix = Matrix4x4.TRS(cameraPos.transform.position, cameraPos.transform.rotation, Vector3.one);
            Gizmos.DrawFrustum(Vector3.zero, currentCamera.fieldOfView, rangeCamera, currentCamera.nearClipPlane, currentCamera.aspect);


            Gizmos.matrix = tempMatrix;
        }
    }
}
