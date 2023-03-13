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
    
    [Header("Autres")]
    [SerializeField] private GameObject node;
    public GameObject origin;
    public GameObject end;
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


    public void CreateNodes(SpringJoint currentSpringOrigin, SpringJoint currentSpringEnd, ObjetInteractible startObject, Rigidbody rigidbodyStart, Rigidbody rigidbodyEnd)
    {
        // Calcule du nombre de nodes à générer (sert plus a rien mais je le laisse au cas où)
        float distanceStartEnd = Vector3.Distance(origin.transform.position, end.transform.position);
        Vector3 directionStartEnd = end.transform.position - origin.transform.position;

        nbrNodes = (int) (distanceStartEnd / distanceBetweenNodes);

        if (nbrNodes > nbrMaxNodes)
            nbrNodes = nbrMaxNodes;
        

        // Creation de chaque node de la corde
        nodesRope.Add(origin);
        
        for (int k = 1; k < nbrNodes + 1; k++)
        {
            Vector3 posNewNode =
                origin.transform.position + (directionStartEnd.normalized * (distanceStartEnd / nbrNodes)) * k;
            
            GameObject newNode = Instantiate(node, posNewNode, Quaternion.identity, transform);
            nodesRope.Add(newNode);
        }
        
        nodesRope.Add(end);


        // Attribution des springs exterieurs au cable (pour la resistance)

        rbOrigin = rigidbodyStart;
        rbEnd = rigidbodyEnd;

        springOrigin = currentSpringOrigin;
        springEnd = currentSpringEnd;
        
        if(springOrigin != null)
            springOrigin.connectedBody = nodesRope[2].GetComponent<Rigidbody>();
        
        if(springEnd != null)
            springEnd.connectedBody = nodesRope[nodesRope.Count - 2].GetComponent<Rigidbody>();

        if(startObject != null)
        {
            startObject.isLinked = true;
            startObject.cable = gameObject;
            startObject.isStart = true;
        }


        CreateCable();
    }

    
    private void CreateCable()
    {
        for(int k = 1; k < nodesRope.Count - 1; k++)
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
        currentNode.node1 = nodesRope[index - 1].transform;
        currentNode.node2 = nodesRope[index + 1].transform;

        currentNode.spring1.connectedBody = nodesRope[index - 1].GetComponent<Rigidbody>();
        currentNode.spring2.connectedBody = nodesRope[index + 1].GetComponent<Rigidbody>();

        
        // GESTION DISTANCE ENTRE POINTS
        currentNode.spring1.anchor = Vector3.zero;
        currentNode.spring2.anchor = Vector3.zero;

        Vector3 direction = nodesRope[index + 1].transform.position - currentNode.transform.position;

        /*currentNode.spring1.anchor = Vector3.zero;
        currentNode.spring2.anchor = -direction.normalized * 0.1f;*/

        currentNode.spring1.connectedAnchor = Vector3.zero;
        currentNode.spring2.connectedAnchor = -direction.normalized * 0.5f;


        // GESTION PHYSIQUE CORDE
        if (isMoved)
        {
            currentNode.spring1.spring = spring;
            currentNode.spring2.spring = spring;
        }
        else
        {
            currentNode.spring1.spring = spring;
            currentNode.spring2.spring = spring;
        }
            
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
        _lineRenderer.positionCount = nodesRope.Count;

        List<Vector3> posLineRenderer = ListePositionsNodes();
        
        _lineRenderer.SetPositions(posLineRenderer.ToArray());
        
        
        // Actualisation de la couleur
        _lineRenderer.material.SetFloat("_GradientSpeed", (currentLength / maxLength));


        // On modifie la puissance des springs des deux extremites en fonction de leur poids et de la longueur du cable
        /*if(springOrigin != null) 
            springOrigin.spring = (multiplicateurResistance + (rbOrigin.mass)) 
                                                       * multiplicateurResistance * (currentLength / maxLength);
        
        if(springEnd != null)
            springEnd.spring = (multiplicateurResistance + (rbEnd.mass)) 
                                                        * multiplicateurResistance * (currentLength / maxLength);*/

        if(currentLength > maxLength)
        {
            if (springOrigin != null)
                springOrigin.spring = rbOrigin.mass * multiplicateurResistance;

            if (springEnd != null)
                springEnd.spring = rbEnd.mass * multiplicateurResistance;
        }
        else
        {
            if (springOrigin != null)
                springOrigin.spring = 0;

            if (springEnd != null)
                springEnd.spring = 0;
        }
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


    // LORSQUE LE CABLE EST PLACÉ QUELQUE PART
    public void ChangeLastNode(GameObject newAnchor, Rigidbody newRb, SpringJoint newSpring)
    {
        GetComponent<Cable>().endOffset = ChooseSpotCable(GetComponent<Cable>().endAnchor, newAnchor) - newAnchor.transform.position;
        GetComponent<Cable>().endAnchor = newAnchor;

        isLinked = true;

        for (int k = 1; k < nodesRope.Count - 1; k++)
        {
            CreateLienBetweenNodes(k, false);
        }

        rbEnd = newRb;
        springEnd = newSpring;

        if (newAnchor.CompareTag("Interactible"))
        {
            ObjetInteractible currentObject = newAnchor.GetComponent<ObjetInteractible>();

            currentObject.isLinked = true;
            currentObject.isStart = false;
            currentObject.cable = gameObject;
        }
    }


    // LANCE UN RAYCAST POUR TROUVER LE MEILLEUR ENDROIT OU PLACER L'EXTREMITE DU CABLE
    public Vector3 ChooseSpotCable(GameObject startObject, GameObject aimedObject)
    {
        Vector3 newDirection = aimedObject.transform.position - startObject.transform.position;
        Vector3 startPos = aimedObject.transform.position - newDirection.normalized * 2;

        RaycastHit raycastHit;

        if (Physics.Raycast(startPos, newDirection.normalized, out raycastHit, 2))
        {
            return raycastHit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }
}
