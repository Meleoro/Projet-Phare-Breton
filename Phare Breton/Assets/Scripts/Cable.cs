using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cable : MonoBehaviour
{
    [HideInInspector] public GameObject originAnchor;
    [HideInInspector] public GameObject endAnchor;

    [HideInInspector] public Vector3 originOffset;
    [HideInInspector] public Vector3 endOffset;

    public Transform originNode;
    public Transform endNode;

    private void Update()
    {
        originNode.position = originAnchor.transform.position + originAnchor.transform.TransformDirection(originOffset);
        endNode.position = endAnchor.transform.position + endAnchor.transform.TransformDirection(endOffset);
    }
}
