using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using Unity.VisualScripting;
using UnityEngine;

public class CableCreator : MonoBehaviour
{
    [Header("Paramètres")] 
    [SerializeField] private int nbrMaxNodes;
    [SerializeField] private float distanceBetweenNodes;
    [SerializeField] private float minDistSpring;
    [SerializeField] private float maxDistSpring;
    [SerializeField] private float spring;
    [SerializeField] private float damper;
    [SerializeField] private float maxLength;
    
    [Header("Autres")]
    [SerializeField] private GameObject node;
    public GameObject origin;
    public GameObject end;
    private LineRenderer _lineRenderer;

    private List<GameObject> nodesRope = new List<GameObject>();
    private float currentLength;
    private int nbrNodes;

    [Header("CouleursCable")]
    public Color cableOkay;
    public Color cableNotokay;


    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }


    private void Update()
    {
        ActualiseCable();
    }


    public void CreateNodes()
    {
        float distanceStartEnd = Vector3.Distance(origin.transform.position, end.transform.position);
        Vector3 directionStartEnd = end.transform.position - origin.transform.position;

        nbrNodes = (int) (distanceStartEnd / distanceBetweenNodes);

        if (nbrNodes > nbrMaxNodes)
            nbrNodes = nbrMaxNodes;

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
            CreateLienBetweenNodes(k);
        }
    }

    
    private void CalculateCableLength()
    {
        currentLength = 0f;
        
        for(int k = 0; k < nodesRope.Count - 1; k++)
        {
            currentLength += Vector3.Distance(nodesRope[k].transform.position, nodesRope[k + 1].transform.position);
        }
    }


    private void CreateNewNode()
    {
        Vector3 newPos = nodesRope[nodesRope.Count - 1].transform.position -
                         nodesRope[nodesRope.Count - 2].transform.position;
        
        GameObject newNode = Instantiate(node, nodesRope[nodesRope.Count - 2].transform.position + newPos, Quaternion.identity, transform);
        
        // Placement dans la liste des nodes de la nouvelle node
        nodesRope.Add(newNode);

        GameObject saveNode = nodesRope[nodesRope.Count - 1];
        nodesRope[nodesRope.Count - 1] = nodesRope[nodesRope.Count - 2];
        nodesRope[nodesRope.Count - 2] = saveNode;
        
        
        // Création du lien avec les nodes adjacentes
        CreateLienBetweenNodes(nodesRope.Count - 2);
        CreateLienBetweenNodes(nodesRope.Count - 3);
    }
    
    
    private void DestroyNewNode()
    {
        Destroy(nodesRope[nodesRope.Count - 2]);
        nodesRope.RemoveAt(nodesRope.Count - 2);
        
        CreateLienBetweenNodes(nodesRope.Count - 2);
    }

    
    private void CreateLienBetweenNodes(int index)
    {
        NodeCable currentNode = nodesRope[index].GetComponent<NodeCable>();

        // CREATION DU LIEN
        currentNode.node1 = nodesRope[index - 1].transform;
        currentNode.node2 = nodesRope[index + 1].transform;

        currentNode.spring1.connectedBody = nodesRope[index - 1].GetComponent<Rigidbody>();
        currentNode.spring2.connectedBody = nodesRope[index + 1].GetComponent<Rigidbody>();

        
        // GESTION DISTANCE ENTRE POINTS
        currentNode.spring1.anchor = Vector3.zero;
        currentNode.spring2.anchor = Vector3.zero;

        Vector3 direction = nodesRope[index + 1].transform.position - currentNode.transform.position;
        
        currentNode.spring1.connectedAnchor = Vector3.zero;
        currentNode.spring2.connectedAnchor = direction.normalized * 0.4f;


        // GESTION PHYSIQUE CORDE
        currentNode.spring1.spring = spring;
        currentNode.spring2.spring = spring * 4;
            
        currentNode.spring1.damper = damper;
        currentNode.spring2.damper = damper;
            
        currentNode.spring1.minDistance = minDistSpring;
        currentNode.spring2.minDistance = minDistSpring;
        
        currentNode.spring1.maxDistance = maxDistSpring;
        currentNode.spring2.maxDistance = maxDistSpring;
    }


    private void ActualiseCable()
    {
        ActualiseLienRenderer();
        
        CalculateCableLength();

        if (currentLength > nodesRope.Count * distanceBetweenNodes * 2 && nodesRope.Count < nbrMaxNodes)
        {
            CreateNewNode();
        }
        
        else if (currentLength < (nodesRope.Count * distanceBetweenNodes) - distanceBetweenNodes)
        {
            DestroyNewNode();
        }
    }

    private void ActualiseLienRenderer()
    {
        // Actualisation de la position
        _lineRenderer.positionCount = nodesRope.Count;

        List<Vector3> posLineRenderer = ListePositionsNodes();
        
        _lineRenderer.SetPositions(posLineRenderer.ToArray());
        
        
        // Actualisation de la couleur
        _lineRenderer.material.color = Color.Lerp(cableOkay, cableNotokay, currentLength / maxLength);
    }

    private List<Vector3> ListePositionsNodes()
    {
        List<Vector3> posLineRenderer = new List<Vector3>();
        
        for (int k = 0; k < nodesRope.Count; k++)
        {
            posLineRenderer.Add(nodesRope[k].transform.position);
        }

        return posLineRenderer;
    }
}
