using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TriggerGrue : MonoBehaviour
{
    [Header("Paramètres")]
    [SerializeField] private List<Transform> positionsGrue;
    [SerializeField] private float grueSpeed;
    [SerializeField] private float distanceTriggerGrue;

    [Header("Références")]
    [SerializeField] private GameObject grueObject;
    private GameObject currentGrue;
    [SerializeField] private BoxCollider trigger;
    [SerializeField] private Animator animGrue;

    [Header("Gizmos")]
    [SerializeField] private bool onlyOnSelected;
    [SerializeField] private Color triggerColor;
    [SerializeField] private Color pathColor;

    [Header("Autres")]
    private bool isActivated;
    private Vector3 currentRotation;
    private bool launchFly;
    private float currentY;
    private int currentIndex;



    private void Start()
    {
        trigger = GetComponent<BoxCollider>();
        currentIndex = 0;

        Initialise();
    }


    private void Update()
    {
        if (!launchFly)
        {
            Vector3 direction = ReferenceManager.Instance.characterReference.transform.position - currentGrue.transform.position;

            RotateGrue(new Vector2(direction.x, direction.z));


            float distance = Vector3.Distance(ReferenceManager.Instance.characterReference.transform.position, currentGrue.transform.position);

            if(distance < distanceTriggerGrue)
            {
                StartCoroutine(MoveGrue(currentIndex));
            }
        }
    }



    private void Initialise()
    {
        if(positionsGrue.Count != 0)
        {
            currentGrue = Instantiate(grueObject, positionsGrue[0].position, Quaternion.identity, transform);

            animGrue = currentGrue.GetComponentInChildren<Animator>();
        }
    }



    IEnumerator MoveGrue(int index)
    {
        Vector3 direction = positionsGrue[index + 1].position - positionsGrue[index].position;

        StartCoroutine(RotateGrueCoroutine(new Vector2(direction.x, direction.z).normalized));

        animGrue.SetTrigger("startVol");

        launchFly = true;
        currentY = transform.position.y;


        yield return new WaitForSeconds(0.9f);


        float distance = Vector3.Distance(positionsGrue[index].position, positionsGrue[index + 1].position);
        float speed = distance / grueSpeed;

        Vector3 midPos = positionsGrue[index].position + direction.normalized * distance * 0.5f;

        currentGrue.transform.DOMoveX(midPos.x, speed);
        currentGrue.transform.DOMoveZ(midPos.z, speed);

        currentGrue.transform.DOMoveY(currentY + 13, speed);


        yield return new WaitForSeconds(speed);


        if (index + 1 >= positionsGrue.Count - 1)
        {
            Destroy(currentGrue);
            Destroy(gameObject);
        }

        currentGrue.transform.DOMoveX(positionsGrue[index + 1].position.x, speed);
        currentGrue.transform.DOMoveZ(positionsGrue[index + 1].position.z, speed);

        currentGrue.transform.DOMoveY(positionsGrue[index + 1].position.y, speed);


        yield return new WaitForSeconds(speed - 1);


        animGrue.SetTrigger("endVol");
        currentIndex += 1;


        yield return new WaitForSeconds(1.5f);


        launchFly = false;
    }


    IEnumerator RotateGrueCoroutine(Vector2 direction)
    {
        float timer = 0.4f;

        Vector3 wantedRotation = new Vector3(direction.x, 0, direction.y);

        while (timer > 0)
        {
            timer -= Time.deltaTime;

            yield return new WaitForSeconds(Time.deltaTime);

            currentRotation = Vector3.Lerp(currentRotation, wantedRotation, Time.deltaTime * 5);

            currentGrue.transform.rotation = Quaternion.LookRotation(currentRotation, Vector3.up);
        }
    }


    private void RotateGrue(Vector2 direction)
    {
        Vector3 wantedRotation = new Vector3(direction.x, 0, direction.y);

        currentRotation = Vector3.Lerp(currentRotation, wantedRotation, Time.deltaTime);

        currentGrue.transform.rotation = Quaternion.LookRotation(currentRotation, Vector3.up);
    }



    /*private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !other.isTrigger && !isActivated)
        {
            isActivated = true;

            StartCoroutine(MoveGrue(0));
        }
    }*/






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
