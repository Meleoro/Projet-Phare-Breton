using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using Unity.VisualScripting;
using UnityEngine;

public class CableCreator : MonoBehaviour
{
    [Header("Paramètres")]
    [SerializeField] private int nbrNodes;
    [SerializeField] private float distanceBetweenNodes;
    [SerializeField] private float spring;
    [SerializeField] private float damper;
    
    [Header("Autres")]
    [SerializeField] private GameObject node;
    public GameObject origin;
    public GameObject end;

    private List<GameObject> nodesRope = new List<GameObject>();

    


    public void CreateNodes()
    {
        float distanceStartEnd = Vector3.Distance(origin.transform.position, end.transform.position);
        Vector3 directionStartEnd = end.transform.position - origin.transform.position;
        
        nodesRope.Add(origin);
        
        for (int k = 0; k < nbrNodes; k++)
        {
            Vector3 posNewNode =
                origin.transform.position + (directionStartEnd.normalized * (distanceStartEnd / nbrNodes)) * k;
            
            GameObject newNode = Instantiate(node, posNewNode, Quaternion.identity, transform);
            nodesRope.Add(newNode);
        }
        
        nodesRope.Add(end);
        
        CreateCable();
    }

    private void CreateCable()
    {
        for(int k = 1; k < nodesRope.Count - 1; k++)
        {
            NodeCable currentNode = nodesRope[k].GetComponent<NodeCable>();

            // CREATION DU LIEN
            currentNode.node1 = nodesRope[k - 1].transform;
            currentNode.node2 = nodesRope[k + 1].transform;

            currentNode.spring1.connectedBody = nodesRope[k - 1].GetComponent<Rigidbody>();
            currentNode.spring2.connectedBody = nodesRope[k + 1].GetComponent<Rigidbody>();
            
            
            // GESTION DISTANCE ENTRE POINTS
            currentNode.spring1.anchor = new Vector3(-distanceBetweenNodes, 0, 0);
            currentNode.spring2.anchor = new Vector3(distanceBetweenNodes * 2, 0, 0);
            
            currentNode.spring1.connectedAnchor = new Vector3(-distanceBetweenNodes * 2, 0, 0);
            currentNode.spring2.connectedAnchor = new Vector3(distanceBetweenNodes * 4, 0, 0);
            
            
            // GESTION PHYSIQUE CORDE
            /*currentNode.spring1.massScale = poidsCable;
            currentNode.spring2.massScale = poidsCable;*/
            
            currentNode.spring1.spring = spring;
            currentNode.spring2.spring = spring * 2;
            
            currentNode.spring1.damper = damper;
            currentNode.spring2.damper = damper * 2;
            
            currentNode.spring2.maxDistance = distanceBetweenNodes;
        }
    }
}
