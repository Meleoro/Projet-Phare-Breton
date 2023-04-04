using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CableCreator : MonoBehaviour
{
    [Header("Paramètres")] 
    public int nbrMaxNodes;
    [SerializeField] private float distanceBetweenNodes;
    [SerializeField] private float spring;
    [SerializeField] private float damper;
    public float maxLength;

    [Header("ResistancePhysique")]
    public float multiplicateurResistance;
    private float currentResistance;
    public SpringJoint springOrigin;
    public SpringJoint springEnd;
    public Rigidbody rbOrigin;
    public Rigidbody rbEnd;
    public bool lockStart;
    public bool lockEnd;
    
    [Header("Autres")]
    [SerializeField] private GameObject node;
    public NodeCable origin;
    public NodeCable end;
    private LineRenderer _lineRenderer;
    private bool isLinked;

    public List<GameObject> nodesRope = new List<GameObject>();
    [HideInInspector] public float currentLength;
    private int nbrNodes;


    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }


    private void Update()
    {
        ActualiseCable();
    }


    public void CreateNodes(SpringJoint currentSpringOrigin, SpringJoint currentSpringEnd, ObjetInteractible startObject, ObjetInteractible endObject, Rigidbody rigidbodyStart, Rigidbody rigidbodyEnd)
    {
        // Calcule du nombre de nodes à générer (sert plus a rien mais je le laisse au cas où)
        float distanceStartEnd = Vector3.Distance(origin.transform.position, end.transform.position);
        Vector3 directionStartEnd = end.transform.position - origin.transform.position;

        nbrNodes = nbrMaxNodes;
        

        // Creation de chaque node de la corde
        nodesRope.Add(origin.gameObject);
        
        for (int k = 1; k < nbrNodes; k++)
        {
            Vector3 posNewNode =
                origin.transform.position + (directionStartEnd.normalized * (distanceStartEnd / nbrNodes)) * k;
            
            GameObject newNode = Instantiate(node, posNewNode, Quaternion.identity, transform);
            nodesRope.Add(newNode);
        }
        
        nodesRope.Add(end.gameObject);


        // Attribution des springs exterieurs au cable (pour la resistance)
        rbOrigin = rigidbodyStart;
        rbEnd = rigidbodyEnd;

        springOrigin = currentSpringOrigin;
        if (springOrigin != null)
        {           
            springOrigin.spring = spring;
            springOrigin.connectedBody = nodesRope[0].GetComponent<Rigidbody>();
        }

        springEnd = currentSpringEnd;
        if (springEnd != null)
        {
            springEnd.spring = spring;
            springEnd.connectedBody = nodesRope[nodesRope.Count - 1].GetComponent<Rigidbody>();
        }
        

        if(startObject != null)
        {
            startObject.isLinked = true;
            startObject.cable = this;
            startObject.isStart = true;
        }

        if (endObject != null)
        {
            endObject.isLinked = true;
            endObject.cable = this;
            endObject.isStart = false;
        }


        CreateCable();
    }

    
    private void CreateCable()
    {
        for (int k = 0; k < nodesRope.Count; k++)
        {
            CreateLienBetweenNodes(k, true);
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
        CreateLienBetweenNodes(nodesRope.Count - 2, false);
        CreateLienBetweenNodes(nodesRope.Count - 3, false);
    }
    
    
    private void DestroyNewNode()
    {
        Destroy(nodesRope[nodesRope.Count - 2]);
        nodesRope.RemoveAt(nodesRope.Count - 2);
        
        //CreateLienBetweenNodes(nodesRope.Count - 2);
    }

    
    private void CreateLienBetweenNodes(int index, bool isMoved)
    {
        NodeCable currentNode = nodesRope[index].GetComponent<NodeCable>();

        // CREATION DU LIEN

        if (index == 0)
        {
            currentNode.node1 = rbOrigin.transform;
            currentNode.node2 = nodesRope[index + 1].transform;
            
            currentNode.spring1.connectedBody = rbOrigin;
            currentNode.spring2.connectedBody = nodesRope[index + 1].GetComponent<Rigidbody>();
        }
        else if (index == nodesRope.Count - 1)
        {
            currentNode.node1 = nodesRope[index - 1].transform;
            currentNode.node2 = rbEnd.transform;
            
            currentNode.spring1.connectedBody = nodesRope[index - 1].GetComponent<Rigidbody>();
            currentNode.spring2.connectedBody = rbEnd;
        }
        else
        {
            currentNode.node1 = nodesRope[index - 1].transform;
            currentNode.node2 = nodesRope[index + 1].transform;

            currentNode.spring1.connectedBody = nodesRope[index - 1].GetComponent<Rigidbody>();
            currentNode.spring2.connectedBody = nodesRope[index + 1].GetComponent<Rigidbody>();
        }


        // GESTION DISTANCE ENTRE POINTS
        currentNode.spring1.anchor = Vector3.zero;
        currentNode.spring2.anchor = Vector3.zero;

        //Vector3 direction = nodesRope[index + 1].transform.position - currentNode.transform.position;

        currentNode.spring1.connectedAnchor = Vector3.zero;
        currentNode.spring2.connectedAnchor = Vector3.zero;
        //currentNode.spring2.connectedAnchor = -direction.normalized * 0.5f;

        /*currentNode.spring1.minDistance = 0.1f;
        currentNode.spring2.minDistance = 0.1f;
        currentNode.spring1.maxDistance = 0.1f;
        currentNode.spring2.maxDistance = 0.1f;*/


        // GESTION PHYSIQUE CORDE

        currentNode.spring1.spring = spring;
        currentNode.spring2.spring = spring;
        
            
        currentNode.spring1.damper = damper;
        currentNode.spring2.damper = damper;
    }


    private void ActualiseCable()
    {
        ActualiseLienRenderer();
        
        CalculateCableLength();

        if (currentLength > nodesRope.Count * distanceBetweenNodes * 2 && nodesRope.Count < nbrMaxNodes)
        {
            CreateNewNode();
        }
        
        /*else if (currentLength < (nodesRope.Count * distanceBetweenNodes) - distanceBetweenNodes)
        {
            DestroyNewNode();
        }*/
    }

    
    private void ActualiseLienRenderer()
    {
        // Actualisation de la position
        _lineRenderer.positionCount = nodesRope.Count + 2;

        List<Vector3> posLineRenderer = ListePositionsNodes();
        
        _lineRenderer.SetPositions(posLineRenderer.ToArray());
        
        
        // Actualisation de la couleur
        _lineRenderer.material.SetFloat("_GradientSpeed", (currentLength / maxLength));
        

        // On modifie la puissance des springs des deux extremites en fonction de leur poids et de la longueur du cable
        if (springOrigin != null && !lockStart)
        {
            origin.spring1.spring = rbOrigin.mass * multiplicateurResistance * ((currentLength / maxLength) * 2);
            origin.spring2.spring = rbOrigin.mass * multiplicateurResistance * ((currentLength / maxLength) * 2);
        }

        else
        {
            origin.spring1.spring = 0;
            origin.spring2.spring = spring;
        }
        
        
        if (springEnd != null && !lockEnd)
        {
            end.spring1.spring = rbEnd.mass * multiplicateurResistance * ((currentLength / maxLength) * 2);
            end.spring2.spring = rbEnd.mass * multiplicateurResistance * ((currentLength / maxLength) * 2);
        }

        else 
        {
            end.spring1.spring = spring;
            end.spring2.spring = 0;
        }
    }


    private List<Vector3> ListePositionsNodes()
    {
        List<Vector3> posLineRenderer = new List<Vector3>();

        if(springOrigin != null)
            posLineRenderer.Add(springOrigin.transform.position);
        
        for (int k = 0; k < nodesRope.Count; k++)
        {
            posLineRenderer.Add(nodesRope[k].transform.position);
        }

        if(springEnd != null)
            posLineRenderer.Add(springEnd.transform.position);

        return posLineRenderer;
    }


    // CHANGE LE POSITION DE LA NODE AU BOUT DU CABLE (FIN DE CABLE)
    public void ChangeLastNode(GameObject newAnchor, Rigidbody newRb, SpringJoint newSpring)
    {
        end.spring2.connectedBody = newRb;

        isLinked = true;

        rbEnd = newRb;
        springEnd = newSpring;
        if (springEnd != null)
            springEnd.spring = spring;

        if (newAnchor.CompareTag("Interactible"))
        {
            ObjetInteractible currentObject = newAnchor.GetComponent<ObjetInteractible>();

            currentObject.isLinked = true;
            currentObject.isStart = false;
            currentObject.cable = this;
        }
    }

    
    // CHANGE LE POSITION DE LA NODE AU BOUT DU CABLE (DEBUT DE CABLE)
    public void ChangeFirstNode(GameObject newAnchor, Rigidbody newRb, SpringJoint newSpring)
    {
        origin.spring1.connectedBody = newRb;

        isLinked = true;

        rbOrigin = newRb;
        springOrigin = newSpring;

        if(springOrigin != null)
            springOrigin.spring = spring;

        if (newAnchor.CompareTag("Interactible"))
        {
            ObjetInteractible currentObject = newAnchor.GetComponent<ObjetInteractible>();

            currentObject.isLinked = true;
            currentObject.isStart = false;
            currentObject.cable = this;
        }
    }
}
