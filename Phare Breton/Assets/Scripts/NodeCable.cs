using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NodeCable : MonoBehaviour
{
    private LineRenderer lineRenderer;

    [SerializeField] private Transform node1;
    [SerializeField] private Transform node2;
    
    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        Vector3[] newPositions = new Vector3[]{node1.position, transform.position, node2.position};
        
        lineRenderer.SetPositions(newPositions);
    }
}
