using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cable : MonoBehaviour
{
    [HideInInspector] public GameObject originAnchor;
    [HideInInspector] public GameObject endAnchor;

    public Transform originNode;
    public Transform endNode;

    private void Update()
    {
        originNode.position = originAnchor.transform.position;
        endNode.position = endAnchor.transform.position;
    }
}
