using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TriggerGrue : MonoBehaviour
{
    [Header("Paramètres")]
    [SerializeField] private List<Transform> positionsGrue;
    [SerializeField] private float grueSpeed;

    [Header("Références")]
    [SerializeField] private GameObject grueObject;
    private GameObject currentGrue;
    [SerializeField] private BoxCollider trigger;

    [Header("Gizmos")]
    [SerializeField] private bool onlyOnSelected;
    [SerializeField] private Color triggerColor;
    [SerializeField] private Color pathColor;

    [Header("Autres")]
    private bool isActivated;



    private void Start()
    {
        trigger = GetComponent<BoxCollider>();

        Initialise();
    }



    private void Initialise()
    {
        if(positionsGrue.Count != 0)
        {
            currentGrue = Instantiate(grueObject, positionsGrue[0].position, Quaternion.identity, transform);
        }
    }



    IEnumerator MoveGrue(int index)
    {
        float distance = Vector3.Distance(positionsGrue[index].position, positionsGrue[index + 1].position);
        float speed = distance / grueSpeed;

        currentGrue.transform.DOMove(positionsGrue[index + 1].position, speed);


        yield return new WaitForSeconds(speed);


        if(index + 1 < positionsGrue.Count - 1)
        {
            StartCoroutine(MoveGrue(index + 1));
        }

        else
        {
            Destroy(currentGrue);
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !other.isTrigger && !isActivated)
        {
            isActivated = true;

            StartCoroutine(MoveGrue(0));
        }
    }






    private void OnDrawGizmos()
    {
        if (!onlyOnSelected)
        {
            Matrix4x4 save = Gizmos.matrix;

            Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.matrix = rotationMatrix;

            Gizmos.color = triggerColor;
            Gizmos.DrawCube(trigger.center, trigger.size);

            Gizmos.matrix = save;


            Gizmos.color = pathColor;

            for (int i = 0; i < positionsGrue.Count - 1; i++)
            {
                Gizmos.DrawSphere(positionsGrue[i].position, 0.2f);
                Gizmos.DrawLine(positionsGrue[i].position, positionsGrue[i + 1].position);
            }

            Gizmos.DrawSphere(positionsGrue[positionsGrue.Count - 1].position, 0.2f);
        }
    }


    private void OnDrawGizmosSelected()
    {
        if (onlyOnSelected)
        {
            Matrix4x4 save = Gizmos.matrix;

            Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.matrix = rotationMatrix;

            Gizmos.color = triggerColor;
            Gizmos.DrawCube(trigger.center, trigger.size);

            Gizmos.matrix = save;


            Gizmos.color = pathColor;

            for (int i = 0; i < positionsGrue.Count - 1; i++)
            {
                Gizmos.DrawSphere(positionsGrue[i].position, 0.2f);
                Gizmos.DrawLine(positionsGrue[i].position, positionsGrue[i + 1].position);
            }

            Gizmos.DrawSphere(positionsGrue[positionsGrue.Count - 1].position, 0.2f);
        }
    }
}
