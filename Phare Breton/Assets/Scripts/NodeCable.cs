using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NodeCable : MonoBehaviour
{
    private LineRenderer lineRenderer;

    public Transform node1;
    public Transform node2;
    
    public SpringJoint spring1;
    public SpringJoint spring2;
    
    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        Vector3[] newPositions = {node1.position, transform.position, node2.position};
        
        lineRenderer.SetPositions(newPositions);
    }
}
