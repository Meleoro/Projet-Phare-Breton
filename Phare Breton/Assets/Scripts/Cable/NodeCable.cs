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

    public FixedJoint joint;
}
