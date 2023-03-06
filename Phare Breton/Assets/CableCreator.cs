using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class CableCreator : MonoBehaviour
{
    [SerializeField] private int nbrNode;
    [SerializeField] private GameObject node;

    [SerializeField] private GameObject origin;
    [SerializeField] private GameObject end;

    private List<GameObject> nodesRope = new List<GameObject>();


    private void Start()
    {
        CreateNodes();
        CreateCable();
    }


    public void CreateNodes()
    {
        float distanceStartEnd = Vector3.Distance(origin.transform.position, end.transform.position);
        Vector3 directionStartEnd = origin.transform.position - end.transform.position;
        
        nodesRope.Add(origin);
        
        for (int k = 0; k < nbrNode; k++)
        {
            Vector3 posNewNode =
                origin.transform.position + (directionStartEnd.normalized * (distanceStartEnd / nbrNode)) * k;
            
            GameObject newNode = Instantiate(node, posNewNode, Quaternion.identity);
            nodesRope.Add(newNode);
        }
        
        nodesRope.Add(end);
    }

    private void CreateCable()
    {
        for(int k = 1; k < nodesRope.Count - 1; k++)
        {
            NodeCable currentNode = nodesRope[k].GetComponent<NodeCable>();

            currentNode.node1 = nodesRope[k - 1].transform;
            currentNode.node2 = nodesRope[k + 1].transform;

            currentNode.spring1.connectedBody = nodesRope[k - 1].GetComponent<Rigidbody>();
            currentNode.spring2.connectedBody = nodesRope[k + 1].GetComponent<Rigidbody>();
        }
    }
}
