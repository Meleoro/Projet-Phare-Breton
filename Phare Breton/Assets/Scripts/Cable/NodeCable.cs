using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeCable : MonoBehaviour
{
    public Transform node1;
    public Transform node2;
    
    public SpringJoint spring1;
    public SpringJoint spring2;

    [Header("New Spring System")]
    public SpringJoint mainJoint;
    public Transform startRope;
    public Transform endRope;

    private void Update()
    {
        //ActualiseNodePos();
    }

    public void ActualiseNodePos()
    {
        Vector3 direction = node1.position - node2.position;
        Vector3 newPos = node2.position + (direction - direction * 2);

        float newHeight =  startRope.transform.position.y + (startRope.transform.position.y - endRope.transform.position.y);

        mainJoint.gameObject.transform.position = new Vector3(newPos.x, newHeight, newPos.z);
    }
}
