using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ZoneSombre : MonoBehaviour
{
    [SerializeField] private List<BoxCollider> zoneColliders;
    [SerializeField] private Color GizmosColor;
    

    private void OnDrawGizmos()
    {
        for (int k = 0; k < zoneColliders.Count; k++)
        {
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(zoneColliders[k].transform.position, zoneColliders[k].transform.rotation, zoneColliders[k].transform.lossyScale);
            Gizmos.matrix = rotationMatrix;

            Gizmos.color = GizmosColor;
            Gizmos.DrawCube(zoneColliders[k].center, zoneColliders[k].size);
        }
    }
}
