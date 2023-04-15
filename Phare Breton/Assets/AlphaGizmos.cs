using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaGizmos : MonoBehaviour
{
    [SerializeField] private bool keepShowing;

    [SerializeField] private EntreePorte scriptPorte;
    [SerializeField] private Transform posCamera;

    [SerializeField] private Color minColor;
    [SerializeField] private Color maxColor;


    public void OnDrawGizmos()
    {
        if (keepShowing)
        {
            Vector3 minPoint = posCamera.position + posCamera.forward * scriptPorte.distanceMin;
            Vector3 maxPoint = posCamera.position + posCamera.forward * scriptPorte.distanceMax;

            Gizmos.color = minColor;
            Gizmos.DrawLine(posCamera.position, minPoint);

            Gizmos.color = maxColor;
            Gizmos.DrawLine(minPoint, maxPoint);
        }
    }


    public void OnDrawGizmosSelected()
    {
        Vector3 minPoint = posCamera.position + posCamera.forward * scriptPorte.distanceMin;
        Vector3 maxPoint = posCamera.position + posCamera.forward * scriptPorte.distanceMax;

        Gizmos.color = minColor;
        Gizmos.DrawLine(posCamera.position, minPoint);

        Gizmos.color = maxColor;
        Gizmos.DrawLine(minPoint, maxPoint);
    }
}
