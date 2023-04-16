using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaGizmos : MonoBehaviour
{
    [Header("Touch")]
    [SerializeField] private bool keepShowing;
    [SerializeField] private bool sphereGizmos;


    [Header("Dont Touch")]
    [SerializeField] private EntreePorte scriptPorte;
    [SerializeField] private Transform posCamera;

    [SerializeField] private Color minColor;
    [SerializeField] private Color maxColor;


    public void OnDrawGizmos()
    {
        if (keepShowing)
        {
            if (sphereGizmos)
            {
                Gizmos.color = minColor;
                Gizmos.DrawWireSphere(posCamera.position, scriptPorte.distanceMin);

                Gizmos.color = maxColor;
                Gizmos.DrawWireSphere(posCamera.position, scriptPorte.distanceMax);
            }

            else
            {
                Vector3 minPoint = posCamera.position + posCamera.forward * scriptPorte.distanceMin;
                Vector3 maxPoint = posCamera.position + posCamera.forward * scriptPorte.distanceMax;

                Gizmos.color = minColor;
                Gizmos.DrawLine(posCamera.position, minPoint);

                Gizmos.color = maxColor;
                Gizmos.DrawLine(minPoint, maxPoint);
            }
        }
    }


    public void OnDrawGizmosSelected()
    {
        if (sphereGizmos)
        {
            Gizmos.color = minColor;
            Gizmos.DrawWireSphere(posCamera.position, scriptPorte.distanceMin);

            Gizmos.color = maxColor;
            Gizmos.DrawWireSphere(posCamera.position, scriptPorte.distanceMax);
        }

        else
        {
            Vector3 minPoint = posCamera.position + posCamera.forward * scriptPorte.distanceMin;
            Vector3 maxPoint = posCamera.position + posCamera.forward * scriptPorte.distanceMax;

            Gizmos.color = minColor;
            Gizmos.DrawLine(posCamera.position, minPoint);

            Gizmos.color = maxColor;
            Gizmos.DrawLine(minPoint, maxPoint);
        }
    }
}
