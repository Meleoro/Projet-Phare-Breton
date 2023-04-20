using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Porte : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] Transform charaPos1;
    [SerializeField] Transform charaPos2;

    [SerializeField] Transform cameraPos1;
    [SerializeField] Transform cameraPos2;

    [SerializeField] EntreePorte door1;
    [SerializeField] EntreePorte door2;

    [Header("Limites Camera")]
    [SerializeField] Transform minXZDoor1;
    [SerializeField] Transform maxXZDoor1;
    [SerializeField] Transform minXZDoor2;
    [SerializeField] Transform maxXZDoor2;

    [Header("Changement Scene")] 
    [SerializeField] private bool changeScene;
    [SerializeField] private string sceneName;

    [Header("Gizmos")] 
    [SerializeField] private bool lineBetweenDoors;
    [SerializeField] private Color lineBetweenDoorsColor;
    [SerializeField] private bool areaCamera;
    [SerializeField] private Color areaCameraColor;
    [SerializeField] private bool cameraView;
    [SerializeField] private Color cameraViewColor;
    [SerializeField] private float rangeCamera;
    private Camera currentCamera;
    
    
    public void EnterDoor(GameObject movedObject, int doorNumber)
    {
        if (changeScene)
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            if (doorNumber == 1)
            {
                ReferenceManager.Instance.cameraReference.ActualiseDesactivatedObjects(door1.desactivatedObjects, door1.distanceMin, door1.distanceMax);
                ReferenceManager.Instance.cameraReference.InitialiseNewZone(minXZDoor2, maxXZDoor2);
            }

            else
            {
                ReferenceManager.Instance.cameraReference.ActualiseDesactivatedObjects(door2.desactivatedObjects, door1.distanceMin, door1.distanceMax);
                ReferenceManager.Instance.cameraReference.InitialiseNewZone(minXZDoor1, maxXZDoor1);
            }

            
            if (movedObject.CompareTag("Interactible"))
            {
                if(doorNumber == 1)
                    movedObject.GetComponent<ObjetInteractible>().currentHauteur = charaPos2.position.y;
                
                else
                    movedObject.GetComponent<ObjetInteractible>().currentHauteur = charaPos1.position.y;

                
                if (movedObject.GetComponent<ObjetInteractible>().isLinked)
                {
                    ObjetInteractible currentObject = movedObject.GetComponent<ObjetInteractible>();

                    if (doorNumber == 1)
                    {
                        if(!door1.hasCableThrough)
                            CableThroughDoor(currentObject.cable.gameObject, movedObject, door1.gameObject, door2.gameObject);
                        
                        else
                            DestroyCableThroughDoor(door1, movedObject);
                    }

                    else
                    {
                        if(!door2.hasCableThrough) 
                            CableThroughDoor(currentObject.cable.gameObject, movedObject, door2.gameObject, door1.gameObject);
                        
                        else
                            DestroyCableThroughDoor(door2, movedObject);
                    }
                }
            }

            if (movedObject.CompareTag("Player"))
            {
                if (movedObject.GetComponent<CharaManager>().hasRope)
                {
                    CharacterFlute currentObject = movedObject.GetComponent<CharacterFlute>();

                    for (int k = currentObject.cables.Count - 1; k >= 0; k--)
                    {
                        if (doorNumber == 1)
                        {
                            if(!door1.hasCableThrough)
                                CableThroughDoor(currentObject.cables[k], movedObject, door1.gameObject, door2.gameObject);
                            
                            else
                                DestroyCableThroughDoor(door1, movedObject);
                        }
                        
                        else
                        {
                            if(!door2.hasCableThrough)
                                CableThroughDoor(currentObject.cables[k], movedObject, door2.gameObject, door1.gameObject);
                            
                            else
                                DestroyCableThroughDoor(door2, movedObject);
                        }
                    }
                }
            }
        }
    }

    
    public void UseDoor(int doorNumber, GameObject movedObject, bool staticCamera)
    {
        if (doorNumber == 1)
        {
            StartCoroutine(ReferenceManager.Instance.cameraReference.scriptFondu.Transition(charaPos2.position, cameraPos2, movedObject, this, doorNumber, staticCamera));
        }

        else
        {
            StartCoroutine(ReferenceManager.Instance.cameraReference.scriptFondu.Transition(charaPos1.position, cameraPos1, movedObject, this, doorNumber, staticCamera));
        }
    }


    public void CableThroughDoor(GameObject currentCable, GameObject currentObject, GameObject endOldCable, GameObject startNewCable)
    {
        // Placement de l'ancien cable
        CableCreator currentScript = currentCable.GetComponent<CableCreator>();

        if (currentObject.CompareTag("Interactible"))
        {
            if(currentObject.GetComponent<ObjetInteractible>().isStart)
                currentScript.ChangeFirstNode(endOldCable, endOldCable.GetComponent<Rigidbody>(), null);
        
            else 
                currentScript.ChangeLastNode(endOldCable, endOldCable.GetComponent<Rigidbody>(), null);
        }
        else
        {
            currentScript.ChangeLastNode(endOldCable, endOldCable.GetComponent<Rigidbody>(), null);
        }

        ReferenceManager.Instance.characterReference.movementScript.resistanceCable = Vector3.zero;
        

        // Recuperation des infos pour le nouveau cable
        float lenghtNewCable = currentScript.maxLength - currentScript.currentLength;
        int nbrNodesNewCable = (int) (currentScript.nbrMaxNodes * (currentScript.currentLength / currentScript.maxLength));


        // CREATION DU SECOND CABLE
        GameObject newCable = Instantiate(ReferenceManager.Instance.characterReference.fluteScript.ropeObject, startNewCable.transform.position, Quaternion.identity);
        CableCreator newScriptCreator = newCable.GetComponent<CableCreator>();
        Cable newScriptCable = newCable.GetComponent<Cable>();

        newScriptCreator.maxLength = lenghtNewCable;
        newScriptCreator.nbrMaxNodes = nbrNodesNewCable;

        newScriptCable.InitialiseStartEnd(currentObject, startNewCable);

        newScriptCreator.CreateNodes(currentObject.GetComponentInChildren<SpringJoint>(), startNewCable.GetComponent<SpringJoint>(), null, null,
            currentObject.GetComponent<Rigidbody>(), startNewCable.GetComponent<Rigidbody>());


        // On assigne tous ces éléments à la porte
        EntreePorte currentDoor = startNewCable.GetComponent<EntreePorte>();

        currentDoor.hasCableThrough = true;
        currentDoor.cableOtherSide = currentScript;
        currentDoor.cableThisSide = newScriptCreator;
    }

    
    public void DestroyCableThroughDoor(EntreePorte doorCrossed, GameObject currentObject)
    {
        if (doorCrossed.hasCableThrough)
        {
            // On détruit la partie du câble qui sert à rien
            Destroy(doorCrossed.cableThisSide.gameObject);
            
            
            // On relie le câble de l'autre côté à l'objet
            if (currentObject.CompareTag("Interactible"))
            {
                if(currentObject.GetComponent<ObjetInteractible>().isStart)
                    doorCrossed.cableOtherSide.ChangeFirstNode(currentObject, currentObject.GetComponent<Rigidbody>(), currentObject.GetComponentInChildren<SpringJoint>());
        
                else 
                    doorCrossed.cableOtherSide.ChangeLastNode(currentObject, currentObject.GetComponent<Rigidbody>(), currentObject.GetComponentInChildren<SpringJoint>());
            }
            else
            {
                doorCrossed.cableOtherSide.ChangeLastNode(currentObject, currentObject.GetComponent<Rigidbody>(), currentObject.GetComponentInChildren<SpringJoint>());
                doorCrossed.cableOtherSide.isLinked = false;
            }
        }
    }


    private void OnDrawGizmosSelected()
    {
        /*if (lineBetweenDoors)
        {
            Gizmos.color = lineBetweenDoorsColor;
            Gizmos.DrawLine(door1.transform.position, door2.transform.position);
        }*/

        if (areaCamera)
        {
            Gizmos.color = areaCameraColor;
            if (!door1.staticCamera)
            {
                Vector3 max = minXZDoor1.transform.InverseTransformPoint(maxXZDoor1.position);
                
                Vector3 point1 = new Vector3(0, 0, max.z);
                Vector3 point2 = new Vector3(max.x, max.y, 0);

                point1 = minXZDoor1.transform.TransformPoint(point1);
                point2 = minXZDoor1.transform.TransformPoint(point2);

                Gizmos.DrawLine(minXZDoor1.position, point1);
                Gizmos.DrawLine(minXZDoor1.position, point2);
                Gizmos.DrawLine(maxXZDoor1.position, point1);
                Gizmos.DrawLine(maxXZDoor1.position, point2);
            }
            
            if (!door2.staticCamera)
            {
                Vector3 max = minXZDoor2.transform.InverseTransformPoint(maxXZDoor2.position);
                
                Vector3 point1 = new Vector3(0, 0, max.z);
                Vector3 point2 = new Vector3(max.x, max.y, 0);

                point1 = minXZDoor2.transform.TransformPoint(point1);
                point2 = minXZDoor2.transform.TransformPoint(point2);

                Gizmos.DrawLine(minXZDoor2.position, point1);
                Gizmos.DrawLine(minXZDoor2.position, point2);
                Gizmos.DrawLine(maxXZDoor2.position, point1);
                Gizmos.DrawLine(maxXZDoor2.position, point2);
            }
        }

        if (cameraView)
        {
            Gizmos.color = cameraViewColor;

            Matrix4x4 tempMatrix = Gizmos.matrix;
            
            if (currentCamera == null)
                currentCamera = ReferenceManager.Instance._camera;

            
            // Camera Pos 1
            Gizmos.matrix = Matrix4x4.TRS(cameraPos1.transform.position, cameraPos1.transform.rotation, Vector3.one);
            Gizmos.DrawFrustum(Vector3.zero, currentCamera.fieldOfView, rangeCamera, currentCamera.nearClipPlane, currentCamera.aspect);
            
            
            // Camera Pos 2
            Gizmos.matrix = Matrix4x4.TRS(cameraPos2.transform.position, cameraPos2.transform.rotation, Vector3.one);
            Gizmos.DrawFrustum(Vector3.zero, currentCamera.fieldOfView, rangeCamera, currentCamera.nearClipPlane, currentCamera.aspect);
            

            Gizmos.matrix = tempMatrix;
        }
    }
}
