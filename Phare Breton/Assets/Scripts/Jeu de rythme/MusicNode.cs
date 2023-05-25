using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class MusicNode : MonoBehaviour
{
    public BandeJeuDeRythme currentBande;

    [HideInInspector] public bool isX;
    [HideInInspector] public bool isY;
    [HideInInspector] public bool isZ;

    [HideInInspector] public bool erased;

    [Header("Movement")] 
    [HideInInspector] public float movementSpeed;
    [HideInInspector] public Vector3 originPos;
    private float timer;
    private int currentIndex;
    private List<RectTransform> waypoints = new List<RectTransform>();

    [Header("References")] 
    [SerializeField] private ParticleSystem VFXDestroy;
    private Image image;

    private RectTransform rectTransform;


    private void Start()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }


    public void InitialiseNode(Node.InputNeeded inputNeeded, Node.SpawnPos spawnPos, BandeJeuDeRythme bande, List<RectTransform> currentWaypoints)
    {
        rectTransform = GetComponent<RectTransform>();
        currentBande = bande;

        movementSpeed = bande.speedMoveNode;
        
        switch (inputNeeded)
        {
            case Node.InputNeeded.x :
                isX = true;
                break;
            
            case Node.InputNeeded.y :
                isY = true;
                break;
            
            case Node.InputNeeded.z :
                isZ = true;
                break;
        }
        
        switch (spawnPos)
        {
            case Node.SpawnPos.left :
                rectTransform.position = bande.posSpawnLeft.position;
                break;
            
            case Node.SpawnPos.right :
                rectTransform.position = bande.posSpawnRight.position;
                break;
        }

        currentIndex = -1;
        timer = 0;

        waypoints = currentWaypoints;
    }


    public void MoveNode(float speed)
    {
        timer -= Time.deltaTime;

        if (timer <= 0) 
        {
            currentIndex += 1;
            float distance = Vector3.Distance(rectTransform.position, waypoints[currentIndex].position);
            float ratio = distance / movementSpeed;
            
            rectTransform.DOMove(waypoints[currentIndex].position, ratio).SetEase(Ease.Linear);
            timer = ratio;
        }
    }
    

    public void EraseNode()
    {
        if (!erased)
        {
            image.enabled = false;
            GetComponentInChildren<TextMeshProUGUI>().enabled = false;
            erased = true;

            currentBande.PlayVFXDestroy();
        }
    }

    public void ReappearNode()
    {
        image.enabled = true;
        GetComponentInChildren<TextMeshProUGUI>().enabled = true;
        erased = false;
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("MusicBarre"))
        {
            currentBande.currentNode = this;

            if (isX)
                currentBande.isOnX = true;

            else if (isY)
                currentBande.isOnY = true;

            else
                currentBande.isOnZ = true;
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("MusicBarre"))
        {
            currentBande.currentNode = null;

            if (isX)
                currentBande.isOnX = false;

            else if (isY)
                currentBande.isOnY = false;

            else
                currentBande.isOnZ = false;


            // Si le joueur n'a pas appuye 
            if (!erased)
            {
                currentBande.RestartGame();
            }
        }
    }
}
