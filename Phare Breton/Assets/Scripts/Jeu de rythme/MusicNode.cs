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
    public float movementSpeed;
    /*public AnimationCurve moveX;
    public AnimationCurve moveY;*/
    [HideInInspector] public Vector3 originPos;
    private float timer;
    private bool goLeft;
    private int currentIndex;
    private List<RectTransform> waypoints = new List<RectTransform>();

    [Header("References")]
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
                goLeft = false;
                break;
            
            case Node.SpawnPos.right :
                rectTransform.position = bande.posSpawnRight.position;
                goLeft = true;
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
        
        
        /*timer += Time.deltaTime * speed;

        if(goLeft) 
            rectTransform.position = originPos + (new Vector3(-moveX.Evaluate(timer), moveY.Evaluate(timer), 0) * 225);
        
        else
            rectTransform.position = originPos + (new Vector3(moveX.Evaluate(timer), moveY.Evaluate(timer), 0) * 225);*/
    }
    

    public void EraseNode()
    {
        image.enabled = false;
        GetComponentInChildren<TextMeshProUGUI>().enabled = false;
        erased = true;
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
